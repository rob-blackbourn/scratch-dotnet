using System;
using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.FeedBus.Messages
{
    public class ForwardedSubscriptionRequest : Message
    {
        public ForwardedSubscriptionRequest(Guid clientId, string feed, string topic, bool isAdd)
            : base(MessageType.ForwardedSubscriptionRequest)
        {
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsAdd { get; }

        public static ForwardedSubscriptionRequest ReadBody(Stream stream)
        {
            var clientId = stream.ReadGuid();
            var feed = stream.ReadString();
            var topic = stream.ReadString();
            var isAdd = stream.ReadBoolean();
            return new ForwardedSubscriptionRequest(clientId, feed, topic, isAdd);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(ClientId);
            stream.Write(Feed);
            stream.Write(Topic);
            stream.Write(IsAdd);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, ClientId={ClientId}, Feed={Feed}, Topic={Topic}, IsAdd{IsAdd}";
        }
    }
}
