using System;
using System.Net;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters
{
    public class ForwardedSubscriptionEventArgs : EventArgs
    {
        public ForwardedSubscriptionEventArgs(string user, IPAddress address, Guid clientId, string feed, string topic, bool isAdd)
        {
            User = user;
            Address = address;
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        public string User { get; }
        public IPAddress Address { get; }
        public Guid ClientId { get; private set; }
        public string Feed { get; private set; }
        public string Topic { get; private set; }
        public bool IsAdd { get; private set; }
    }
}
