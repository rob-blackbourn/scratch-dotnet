using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Adapters
{
    public class ReconnectingClient : IClient
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Func<Client> _clientFactory;
        private readonly TimeSpan _reconnectInterval;
        private readonly int _maxRetries;
        private Client _client;

        private readonly object _gate = new object();
        private bool _isFaulted;
        private readonly IList<FeedTopic> _subscriptions = new List<FeedTopic>();
        private readonly IList<string> _monitors = new List<string>();
        private readonly IList<string> _notifications = new List<string>();

        public ReconnectingClient(Func<Client> clientFactory, TimeSpan reconnectInterval, int maxRetries)
        {
            _clientFactory = clientFactory;
            _reconnectInterval = reconnectInterval;
            _maxRetries = maxRetries;

            try
            {
                _client = clientFactory();
                AttachEvents(_client);
            }
            catch
            {
                Reconnect();
            }
        }

        private void AttachEvents(Client client)
        {
            if (client == null) return;

            client.OnConnectionChanged += ConnectionChanged;
            client.OnDataError += DataError;
            client.OnDataReceived += DataReceived;
            client.OnForwardedSubscription += ForwardedSubscription;
            client.OnHeartbeat += Heartbeat;

            foreach (var feedTopic in _subscriptions)
                client.AddSubscription(feedTopic.Feed, feedTopic.Topic);
            foreach (var feed in _monitors)
                client.AddMonitor(feed);
            foreach (var feed in _notifications)
                client.AddNotification(feed);
        }

        private void DetachEvents(Client client)
        {
            if (client == null) return;

            client.OnConnectionChanged -= ConnectionChanged;
            client.OnDataError -= DataError;
            client.OnDataReceived -= DataReceived;
            client.OnForwardedSubscription -= ForwardedSubscription;
        }

        public event EventHandler<DataReceivedEventArgs> OnDataReceived;
        public event EventHandler<DataErrorEventArgs> OnDataError;
        public event EventHandler<ForwardedSubscriptionEventArgs> OnForwardedSubscription;
        public event EventHandler<ConnectionChangedEventArgs> OnConnectionChanged;
        public event EventHandler<EventArgs> OnHeartbeat;

        public void AddSubscription(string feed, string topic)
        {
            try
            {
                lock (_gate)
                {
                    _subscriptions.Add(new FeedTopic(feed, topic));
                    _client.AddSubscription(feed, topic);
                }
            }
            catch
            {
                Reconnect();
            }
        }

        public void RemoveSubscription(string feed, string topic)
        {
            try
            {
                lock (_gate)
                {
                    _subscriptions.Remove(new FeedTopic(feed, topic));
                    _client.RemoveSubscription(feed, topic);
                }
            }
            catch
            {
                Reconnect();
            }
        }


        public void AddMonitor(string feed)
        {
            try
            {
                lock (_gate)
                {
                    _monitors.Add(feed);
                    _client.AddMonitor(feed);
                }

            }
            catch
            {
                Reconnect();
            }
        }

        public void RemoveMonitor(string feed)
        {
            try
            {
                lock (_gate)
                {
                    _monitors.Remove(feed);
                    _client.RemoveMonitor(feed);
                }
            }
            catch
            {
                Reconnect();
            }
        }


        public void AddNotification(string feed)
        {
            try
            {
                lock (_gate)
                {
                    _notifications.Add(feed);
                    _client.AddNotification(feed);
                }
            }
            catch
            {
                Reconnect();
            }
        }

        public void RemoveNotification(string feed)
        {
            try
            {
                lock (_gate)
                {
                    _notifications.Remove(feed);
                    _client.RemoveNotification(feed);
                }
            }
            catch
            {
                Reconnect();
            }
        }

        public void Send(Guid clientId, string feed, string topic, bool isImage, object data)
        {
            try
            {
                lock (_gate)
                {
                    _client.Send(clientId, feed, topic, isImage, data);
                }
            }
            catch
            {
                Reconnect();
            }
        }

        public void Publish(string feed, string topic, bool isImage, object data)
        {
            try
            {
                lock (_gate)
                {
                    _client.Publish(feed, topic, isImage, data);
                }
            }
            catch
            {
                Reconnect();
            }
        }

        public void Dispose()
        {
            try
            {
                _client?.Dispose();
            }
            catch
            {
                // Ignore
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs args)
        {
            OnDataReceived?.Invoke(this, args);
        }

        private void DataError(object sender, DataErrorEventArgs args)
        {
            OnDataError?.Invoke(this, args);
        }

        private void ForwardedSubscription(object sender, ForwardedSubscriptionEventArgs args)
        {
            OnForwardedSubscription?.Invoke(this, args);
        }

        private void ConnectionChanged(object sender, ConnectionChangedEventArgs args)
        {
            OnConnectionChanged?.Invoke(this, args);

            if (args.State == ConnectionState.Faulted)
                Reconnect();
        }

        private void Heartbeat(object sender, EventArgs args)
        {
            OnHeartbeat?.Invoke(this, args);
        }

        private void Reconnect()
        {
            lock (_gate)
            {
                if (_isFaulted)
                    return;

                _isFaulted = true;
                DetachEvents(_client);
                _client = null;

                Task.Run((Action)ReconnectLoop);
            }
        }

        private void ReconnectLoop()
        {
            var count = _maxRetries;

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Token.WaitHandle.WaitOne(_reconnectInterval);

                try
                {
                    lock (_gate)
                    {
                        OnConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(ConnectionState.Connecting));
                        _client = _clientFactory();
                        AttachEvents(_client);
                        _isFaulted = false;
                        OnConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(ConnectionState.Connected));
                    }
                    return;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception error)
                {
                    OnConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(ConnectionState.Faulted, error));

                    if (--count < 0)
                        break;
                }
            }
        }
    }
}
