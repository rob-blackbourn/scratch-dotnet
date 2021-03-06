﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;
using JetBlack.MessageBus.AuthFeedBus.Messages;
using log4net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public class Interactor : IInteractor
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly BlockingCollection<Message> _writeQueue = new BlockingCollection<Message>();
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly Stream _stream;
        private readonly CancellationToken _token;
        private readonly RoleManager _roleManager;

        public static IInteractor Create(TcpClient tcpClient, DistributorRole distributorRole, EventQueue<InteractorEventArgs> eventQueue, CancellationToken token)
        {
            var stream = new NegotiateStream(tcpClient.GetStream());
            try
            {
                stream.AuthenticateAsServer();
            }
            catch (Exception error)
            {
                Log.Warn("Failed to negotiate stream", error);
                return null;
            }

            var interactor = new Interactor(stream, stream.RemoteIdentity.Name, ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address, distributorRole, eventQueue, token);
            return interactor;
        }

        private Interactor(Stream stream, string name, IPAddress address, DistributorRole distributorRole, EventQueue<InteractorEventArgs> eventQueue, CancellationToken token)
        {
            _stream = stream;
            Id = Guid.NewGuid();
            User = name;
            Address = address;
            _token = token;
            _eventQueue = eventQueue;
            _roleManager = new RoleManager(distributorRole, address, name);
        }

        public Guid Id { get; }
        public string User { get; }
        public IPAddress Address { get; }

        public void Start()
        {
            Task.Run(() => QueueReceivedMessages(), _token);
            Task.Run(() => WriteQueuedMessages(), _token);
        }

        private void QueueReceivedMessages()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    _eventQueue.Enqueue(new InteractorMessageEventArgs(this, ReceiveMessage()));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception error)
                {
                    _eventQueue.Enqueue(new InteractorErrorEventArgs(this, error));
                    break;
                }
            }

            Log.Debug($"Exited read loop for {this}");
        }

        private void WriteQueuedMessages()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var message = _writeQueue.Take(_token);
                    message.Write(_stream);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception error)
                {
                    _eventQueue.Enqueue(new InteractorErrorEventArgs(this, error));
                    break;
                }
            }

            Log.Debug($"Exited write loop for {this}");
        }

        public void SendMessage(Message message)
        {
            _writeQueue.Add(message, _token);
        }

        public Message ReceiveMessage()
        {
            return Message.Read(_stream);
        }

        public bool HasRole(string feed, Role role)
        {
            return _roleManager.HasRole(feed, role);
        }

        public int CompareTo(IInteractor other)
        {
            return other == null ? 1 : Id.CompareTo(other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Interactor);
        }

        public bool Equals(IInteractor other)
        {
            return other != null && other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}: {User}@{Address}";
        }

        public void Dispose()
        {
            _stream.Close();
        }

        public static bool operator ==(Interactor a, Interactor b)
        {
            return (ReferenceEquals(a, null) && ReferenceEquals(b, null)) || (!ReferenceEquals(a, null) && a.Equals(b));
        }

        public static bool operator !=(Interactor a, Interactor b)
        {
            return !(a == b);
        }
    }
}
