using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using log4net;

namespace JetBlack.MessageBus.FeedBus.Distributor
{
    public class Acceptor
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly CancellationToken _token;
        private readonly IPEndPoint _endPoint;

        public Acceptor(IPEndPoint endPoint, EventQueue<InteractorEventArgs> eventQueue, CancellationToken token)
        {
            _eventQueue = eventQueue;
            _token = token;
            _endPoint = endPoint;
        }

        public void Start(Func<IPEndPoint, EventQueue<InteractorEventArgs>, CancellationToken, IInteractorListener> listenerFactory)
        {
            Task.Factory.StartNew(() => Accept(listenerFactory), _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void Accept(Func<IPEndPoint, EventQueue<InteractorEventArgs>, CancellationToken, IInteractorListener> listenerFactory)
        {
            var listener = listenerFactory(_endPoint, _eventQueue, _token);

            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var interactor = listener.Accept();
                    _eventQueue.Enqueue(new InteractorConnectedEventArgs(interactor));
                }
                catch (Exception error)
                {
                    Log.Warn("Failed to accept interactor", error);
                    throw;
                }
            }
        }
    }
}
