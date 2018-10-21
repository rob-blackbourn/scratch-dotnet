using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public class InteractorListener : IInteractorListener, IDisposable
    {
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly CancellationToken _token;
        private readonly TcpListener _listener;

        public InteractorListener(IPEndPoint endPoint, EventQueue<InteractorEventArgs> eventQueue, CancellationToken token)
        {
            _eventQueue = eventQueue;
            _token = token;
            _listener = new TcpListener(endPoint);
            _listener.Start();
        }

        public void Dispose()
        {
            _listener.Stop();
        }

        public IInteractor Accept()
        {
            var tcpClient = _listener.AcceptTcpClient();
            return Interactor.Create(tcpClient, _eventQueue, _token);
        }
    }
}
