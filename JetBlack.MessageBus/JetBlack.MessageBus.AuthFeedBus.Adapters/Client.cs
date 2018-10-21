using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.AuthFeedBus.Adapters.Configuration;
using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters
{
    public class Client : IDisposable
    {
        public static Client Create(string name)
        {
            var configSection = AuthFeedBusSection.GetSection();
            var config = configSection.Connections[name];
            var endPoint = new IPEndPoint(config.Address, config.Port);
            var byteEncoder = (IByteEncoder)Activator.CreateInstance(config.ByteEncoderType);
            return Create(endPoint, byteEncoder);
        }

        public static Client Create(IPEndPoint endpoint)
        {
            return Create(endpoint, new BinaryEncoder());
        }

        public static Client Create(IPEndPoint endpoint, IByteEncoder byteEncoder)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(endpoint.Address, endpoint.Port);

            var stream = new NegotiateStream(tcpClient.GetStream(), false);
            stream.AuthenticateAsClient();

            var client = new Client(stream, byteEncoder);
            client.Start();
            return client;
        }

        public event EventHandler<InteractorAvertisementEventArgs> OnInteractorAdvertisement;
        public event EventHandler<DataReceivedEventArgs> OnDataReceived;
        public event EventHandler<DataErrorEventArgs> OnDataError;
        public event EventHandler<ForwardedSubscriptionEventArgs> OnForwardedSubscription;
        public event EventHandler<AuthorizationRequestEventArgs> OnAuthorizationRequest;
        public event EventHandler<ConnectionChangedEventArgs> OnConnectionChanged;
        public event EventHandler<EventArgs> OnHeartbeat;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Stream _stream;
        private readonly IByteEncoder _byteEncoder;
        private readonly BlockingCollection<Message> _writeQueue = new BlockingCollection<Message>();

        public Client(Stream stream, IByteEncoder byteEncoder)
        {
            _stream = stream;
            _byteEncoder = byteEncoder;
        }

        public void Start()
        {
            Task.Run(() => Read(), _cancellationTokenSource.Token);
            Task.Run(() => Write(), _cancellationTokenSource.Token);
        }

        private void Read()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var message = Message.Read(_stream);

                    switch (message.MessageType)
                    {
                        case MessageType.InteractorAdvertisement:
                            RaiseOnInteractorAdvertisement((InteractorAdvertisement) message);
                            break;
                        case MessageType.AuthorizationRequest:
                            RaiseOnAuthorizationRequest((AuthorizationRequest)message);
                            break;
                        case MessageType.ForwardedMulticastData:
                            RaiseOnDataOrHeartbeat((ForwardedMulticastData)message);
                            break;
                        case MessageType.ForwardedUnicastData:
                            RaiseOnData((ForwardedUnicastData)message);
                            break;
                        case MessageType.ForwardedSubscriptionRequest:
                            RaiseOnForwardedSubscriptionRequest((ForwardedSubscriptionRequest)message);
                            break;
                        default:
                            throw new ArgumentException("invalid message type");
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (EndOfStreamException)
                {
                    RaiseOnConnectionChanged(ConnectionState.Closed);
                    return;
                }
                catch (Exception error)
                {
                    RaiseOnConnectionChanged(ConnectionState.Faulted, error);
                    return;
                }
            }
        }

        private void Write()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var message = _writeQueue.Take(_cancellationTokenSource.Token);
                    message.Write(_stream);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (EndOfStreamException)
                {
                    RaiseOnConnectionChanged(ConnectionState.Closed);
                    return;
                }
                catch (Exception error)
                {
                    RaiseOnConnectionChanged(ConnectionState.Faulted, error);
                    return;
                }
            }
        }

        public void AddSubscription(string feed, string topic)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new SubscriptionRequest(feed, topic, true));
        }

        public void RemoveSubscription(string feed, string topic)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new SubscriptionRequest(feed, topic, false));
        }

        private BinaryDataPacket[] Encode(IReadOnlyList<DataPacket> data)
        {
            if (data == null)
                return null;

            var encoded = new BinaryDataPacket[data.Count];
            for (var i = 0; i < data.Count; ++i)
                encoded[i] = new BinaryDataPacket(data[i].Header, _byteEncoder.Encode(data[i].Body));
            return encoded;
        }

        public void Send(Guid clientId, string feed, string topic, bool isImage, IReadOnlyList<DataPacket> data)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            try
            {
                _writeQueue.Add(new UnicastData(clientId, feed, topic, isImage, Encode(data)));
            }
            catch (Exception error)
            {
                OnDataError?.Invoke(this, new DataErrorEventArgs(true, feed, topic, isImage, data, error));
            }
        }

        public void Publish(string feed, string topic, bool isImage, IReadOnlyList<DataPacket> data)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            try
            {
                _writeQueue.Add(new MulticastData(feed, topic, isImage, Encode(data)));
            }
            catch (Exception error)
            {
                OnDataError?.Invoke(this, new DataErrorEventArgs(true, feed, topic, isImage, data, error));
            }
        }

        public void Authorise(Guid clientId, string feed, string topic, bool isAuthorizationRequired, Guid[] entitlements)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new AuthorizationResponse(clientId, feed, topic, isAuthorizationRequired, entitlements));
        }

        public void AddNotification(string feed)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));

            _writeQueue.Add(new NotificationRequest(feed, true));
        }

        public void RemoveNotification(string feed)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));

            _writeQueue.Add(new NotificationRequest(feed, false));
        }

        private void RaiseOnInteractorAdvertisement(InteractorAdvertisement message)
        {
            OnInteractorAdvertisement?.Invoke(this, new InteractorAvertisementEventArgs(message.User, message.Address, message.IsJoining));
        }

        private void RaiseOnAuthorizationRequest(AuthorizationRequest message)
        {
            OnAuthorizationRequest?.Invoke(this, new AuthorizationRequestEventArgs(message.ClientId, message.Address, message.User, message.Feed, message.Topic));
        }

        private void RaiseOnForwardedSubscriptionRequest(ForwardedSubscriptionRequest message)
        {
            OnForwardedSubscription?.Invoke(this, new ForwardedSubscriptionEventArgs(message.User, message.Address, message.ClientId, message.Feed, message.Topic, message.IsAdd));
        }

        private void RaiseOnDataOrHeartbeat(ForwardedMulticastData message)
        {
            if (message.Feed == "__admin__" && message.Topic == "heartbeat")
                RaiseOnHeartbeat();
            else
                RaiseOnData(message.User, message.Address, message.Feed, message.Topic, message.Data, message.IsImage);
        }

        private void RaiseOnHeartbeat()
        {
            OnHeartbeat?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseOnData(ForwardedUnicastData message)
        {
            RaiseOnData(message.User, message.Address, message.Feed, message.Topic, message.Data, message.IsImage);
        }

        protected DataPacket[] Decode(BinaryDataPacket[] data)
        {
            if (data == null)
                return null;

            var decoded = new DataPacket[data.Length];
            for (int i = 0; i < data.Length; ++i)
                decoded[i] = new DataPacket(data[i].Header, _byteEncoder.Decode(data[i].Body));

            return decoded;
        }

        private void RaiseOnData(string user, IPAddress address, string feed, string topic, BinaryDataPacket[] data, bool isImage)
        {
            try
            {
                OnDataReceived?.Invoke(this, new DataReceivedEventArgs(user, address, feed, topic, Decode(data), isImage));
            }
            catch (Exception error)
            {
                OnDataError?.Invoke(this, new DataErrorEventArgs(false, feed, topic, isImage, data, error));
            }
        }

        private void RaiseOnConnectionChanged(ConnectionState state, Exception error = null)
        {
            OnConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(state, error));
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _stream.Close();
        }
    }
}
