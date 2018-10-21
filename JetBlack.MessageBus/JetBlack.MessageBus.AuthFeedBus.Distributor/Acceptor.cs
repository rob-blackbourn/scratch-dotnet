using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;
using log4net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor
{
    public class Acceptor
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly CancellationToken _token;
        private readonly IPEndPoint _endPoint;
        private readonly DistributorRole _distributorRole;

        public Acceptor(IPEndPoint endPoint, DistributorRole distributorRole, EventQueue<InteractorEventArgs> eventQueue, CancellationToken token)
        {
            _eventQueue = eventQueue;
            _token = token;
            _distributorRole = distributorRole;
            _endPoint = endPoint;
        }

        public void Start()
        {
            Task.Run(() => Accept(), _token);
        }

        private void Accept()
        {
            var listener = new TcpListener(_endPoint);
            listener.Start();

            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = listener.AcceptTcpClient();
                    var interactor = Interactor.Create(tcpClient, _distributorRole, _eventQueue, _token);
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
