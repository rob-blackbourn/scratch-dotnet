using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.FeedBus.Adapters.Configuration;
using JetBlack.MessageBus.FeedBus.Messages;
using Microsoft.Extensions.Configuration;

namespace JetBlack.MessageBus.FeedBus.Adapters
{
    public class Client : IClient
    {
        public static Client Create(string name)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .Get<FeedBusConfig>();

            var connectionConfig = config.Connections[name];
            var endPoint = new IPEndPoint(connectionConfig.Address, connectionConfig.Port);
            var byteEncoder = (IByteEncoder)Activator.CreateInstance(connectionConfig.ByteEncoderType);
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

            var client = new Client(tcpClient.GetStream(), byteEncoder);
            client.Start();

            client.AddSubscription("_admin", "heartbeat");

            return client;
        }

        public event EventHandler<DataReceivedEventArgs> OnDataReceived;
        public event EventHandler<DataErrorEventArgs> OnDataError;
        public event EventHandler<ForwardedSubscriptionEventArgs> OnForwardedSubscription;
        public event EventHandler<ConnectionChangedEventArgs> OnConnectionChanged;
        public event EventHandler<EventArgs> OnHeartbeat;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Stream _stream;
        private readonly IByteEncoder _byteEncoder;
        private readonly BlockingCollection<Message> _writeQueue = new BlockingCollection<Message>();

        internal Client(Stream stream, IByteEncoder byteEncoder)
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
                        case MessageType.MulticastData:
                            RaiseOnDataOrHeartbeat((MulticastData)message);
                            break;
                        case MessageType.UnicastData:
                            RaiseOnData((UnicastData)message);
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
                    RaiseConnectionStateChanged(ConnectionState.Closed);
                    return;
                }
                catch (Exception error)
                {
                    RaiseConnectionStateChanged(ConnectionState.Faulted, error);
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
                    RaiseConnectionStateChanged(ConnectionState.Closed);
                    return;
                }
                catch (Exception error)
                {
                    RaiseConnectionStateChanged(ConnectionState.Faulted, error);
                    return;
                }
            }
        }

        private void RaiseConnectionStateChanged(ConnectionState state, Exception error = null)
        {
            OnConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(state, error));
        }

        public void AddSubscription(string feed, string topic)
        {
            MakeSubscriptionRequest(feed, topic, true);
        }

        public void RemoveSubscription(string feed, string topic)
        {
            MakeSubscriptionRequest(feed, topic, false);
        }

        private void MakeSubscriptionRequest(string feed, string topic, bool isAdd)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new SubscriptionRequest(feed, topic, isAdd));
        }

        public void AddMonitor(string feed)
        {
            MakeMonitorRequest(feed, true);
        }

        public void RemoveMonitor(string feed)
        {
            MakeMonitorRequest(feed, false);
        }

        private void MakeMonitorRequest(string feed, bool isAdd)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));

            _writeQueue.Add(new MonitorRequest(feed, isAdd));
        }

        public void AddNotification(string feed)
        {
            MakeNotificationRequest(feed, true);
        }

        public void RemoveNotification(string feed)
        {
            MakeNotificationRequest(feed, false);
        }

        private void MakeNotificationRequest(string feed, bool isAdd)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));

            _writeQueue.Add(new NotificationRequest(feed, isAdd));
        }

        public void Send(Guid clientId, string feed, string topic, bool isImage, object data)
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

        public void Publish(string feed, string topic, bool isImage, object data)
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

        private void RaiseOnForwardedSubscriptionRequest(ForwardedSubscriptionRequest message)
        {
            OnForwardedSubscription?.Invoke(this, new ForwardedSubscriptionEventArgs(message.ClientId, message.Feed, message.Topic, message.IsAdd));
        }

        private void RaiseOnDataOrHeartbeat(MulticastData message)
        {
            if (message.Feed == "__admin__" && message.Topic == "heartbeat")
                RaiseOnHeartbeat();
            else
                RaiseOnData(message.Feed, message.Topic, message.Data, message.IsImage);
        }

        private void RaiseOnHeartbeat()
        {
            OnHeartbeat?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseOnData(UnicastData message)
        {
            RaiseOnData(message.Feed, message.Topic, message.Data, message.IsImage);
        }

        protected byte[] Encode(object data)
        {
            return data == null ? null : _byteEncoder.Encode(data);
        }

        protected object Decode(byte[] data)
        {
            return _byteEncoder.Decode(data);
        }

        private void RaiseOnData(string feed, string topic, byte[] data, bool isImage)
        {
            try
            {
                OnDataReceived?.Invoke(this, new DataReceivedEventArgs(feed, topic, Decode(data), isImage));
            }
            catch (Exception error)
            {
                OnDataError?.Invoke(this, new DataErrorEventArgs(false, feed, topic, isImage, data, error));
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _stream.Close();
        }
    }
}
