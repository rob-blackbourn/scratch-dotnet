using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Distributor.Notifiers;
using JetBlack.MessageBus.FeedBus.Distributor.Subscribers;
using JetBlack.MessageBus.FeedBus.Messages;
using log4net;

namespace JetBlack.MessageBus.FeedBus.Distributor
{
    public class Server : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly Acceptor _acceptor;
        private readonly Timer _heartbeatTimer;

        private readonly IInteractorManager _interactorManager;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly INotificationManager _notificationManager;

        public Server(IPEndPoint endPoint)
        {
            _eventQueue = new EventQueue<InteractorEventArgs>(_cancellationTokenSource.Token);
            _eventQueue.OnItemDequeued += OnInteractorEvent;

            _heartbeatTimer = new Timer(HeartbeatCallback);

            _acceptor = new Acceptor(endPoint, _eventQueue, _cancellationTokenSource.Token);

            _interactorManager = new InteractorManager();

            _notificationManager = new NotificationManager(_interactorManager);

            _subscriptionManager = new SubscriptionManager(_interactorManager, _notificationManager);
        }

        public void Start(TimeSpan heartbeatInterval)
        {
            Log.Info($"Starting server {Assembly.GetExecutingAssembly().GetName().Version}");

            _eventQueue.Start();
            _acceptor.Start((endpoint, eventQueue, token) => new InteractorListener(endpoint, eventQueue, token));

            if (heartbeatInterval != TimeSpan.Zero)
                _heartbeatTimer.Change(heartbeatInterval, heartbeatInterval);

            Log.Info("Server started");
        }

        private void OnInteractorEvent(object sender, InteractorEventArgs args)
        {
            if (args is InteractorConnectedEventArgs)
                OnInteractorConnected((InteractorConnectedEventArgs)args);
            else if (args is InteractorMessageEventArgs)
                OnMessage((InteractorMessageEventArgs)args);
            else if (args is InteractorErrorEventArgs)
                OnInteractorError((InteractorErrorEventArgs)args);
        }

        private void OnInteractorConnected(InteractorConnectedEventArgs args)
        {
            _interactorManager.AddInteractor(args.Interactor);
            args.Interactor.Start();
        }

        private void OnInteractorError(InteractorErrorEventArgs args)
        {
            if (args.Error is EndOfStreamException)
                _interactorManager.CloseInteractor(args.Interactor);
            else
                _interactorManager.FaultInteractor(args.Interactor, args.Error);
        }

        private void OnMessage(InteractorMessageEventArgs args)
        {
            Log.Debug($"OnMessage(sender={args.Interactor}, message={args.Message}");

            switch (args.Message.MessageType)
            {
                case MessageType.MonitorRequest:
                    _subscriptionManager.RequestMonitor(args.Interactor, (MonitorRequest)args.Message);
                    break;

                case MessageType.SubscriptionRequest:
                    _subscriptionManager.RequestSubscription(args.Interactor, (SubscriptionRequest)args.Message);
                    break;

                case MessageType.MulticastData:
                    _subscriptionManager.SendMulticastData(args.Interactor, (MulticastData)args.Message);
                    break;

                case MessageType.UnicastData:
                    _subscriptionManager.SendUnicastData(args.Interactor, (UnicastData)args.Message);
                    break;

                case MessageType.NotificationRequest:
                    _notificationManager.RequestNotification(args.Interactor, (NotificationRequest)args.Message);
                    break;

                default:
                    Log.Warn($"Received unknown message type {args.Message.MessageType} from interactor {args.Interactor}.");
                    break;
            }
        }

        private void HeartbeatCallback(object state)
        {
            Log.Debug("Sending heartbeat");
            _eventQueue.Enqueue(new InteractorMessageEventArgs(null, new MulticastData("__admin__", "heartbeat", true, null)));
        }

        public void Dispose()
        {
            Log.Info("Stopping server");

            _heartbeatTimer.Dispose();

            _cancellationTokenSource.Cancel();

            Log.Info("Server stopped");
        }
    }
}
