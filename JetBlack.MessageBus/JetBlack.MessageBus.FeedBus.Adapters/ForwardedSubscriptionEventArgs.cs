using System;

namespace JetBlack.MessageBus.FeedBus.Adapters
{
    public class ForwardedSubscriptionEventArgs : EventArgs
    {
        public ForwardedSubscriptionEventArgs(Guid clientId, string feed, string topic, bool isAdd)
        {
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        public Guid ClientId { get; private set; }
        public string Feed { get; private set; }
        public string Topic { get; private set; }
        public bool IsAdd { get; private set; }
    }
}
