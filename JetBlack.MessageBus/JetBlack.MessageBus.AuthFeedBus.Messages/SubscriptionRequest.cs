using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class SubscriptionRequest : Message
    {
        public SubscriptionRequest(string feed, string topic, bool isAdd)
            : base(MessageType.SubscriptionRequest)
        {
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        public string Feed { get; }
        public string Topic { get; }
        public bool IsAdd { get; }

        public static SubscriptionRequest ReadBody(Stream stream)
        {
            var feed = stream.ReadString();
            var topic = stream.ReadString();
            var isAdd = stream.ReadBoolean();
            return new SubscriptionRequest(feed, topic, isAdd);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(Feed);
            stream.Write(Topic);
            stream.Write(IsAdd);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Feed={Feed}, Topic={Topic}, IsAdd={IsAdd}";
        }
    }
}
