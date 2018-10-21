using System;
using System.Net;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters
{
    public class AuthorizationRequestEventArgs : EventArgs
    {
        public AuthorizationRequestEventArgs(Guid clientId, IPAddress address, string user, string feed, string topic)
        {
            ClientId = clientId;
            Address = address;
            User = user;
            Feed = feed;
            Topic = topic;
        }

        public Guid ClientId { get; private set; }
        public IPAddress Address { get; private set; }
        public string User { get; private set; }
        public string Feed { get; private set; }
        public string Topic { get; private set; }
    }
}
