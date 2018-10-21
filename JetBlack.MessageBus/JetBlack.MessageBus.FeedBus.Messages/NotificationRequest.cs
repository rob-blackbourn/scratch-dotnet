using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.FeedBus.Messages
{
    public class NotificationRequest : Message
    {
        public NotificationRequest(string feed, bool isAdd)
            : base(MessageType.NotificationRequest)
        {
            Feed = feed;
            IsAdd = isAdd;
        }

        public string Feed { get; }
        public bool IsAdd { get; }

        public static NotificationRequest ReadBody(Stream stream)
        {
            var feed = stream.ReadString();
            var isAdd = stream.ReadBoolean();
            return new NotificationRequest(feed, isAdd);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(Feed);
            stream.Write(IsAdd);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Feed={Feed}, IsAdd={IsAdd}";
        }
    }
}
