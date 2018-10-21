using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.FeedBus.Messages
{
    public abstract class Message
    {
        protected Message(MessageType messageType)
        {
            MessageType = messageType;
        }

        public MessageType MessageType { get; }

        public static Message Read(Stream stream)
        {
            var messageType = ReadHeader(stream);

            switch (messageType)
            {
                case MessageType.MulticastData:
                    return MulticastData.ReadBody(stream);
                case MessageType.UnicastData:
                    return UnicastData.ReadBody(stream);
                case MessageType.ForwardedSubscriptionRequest:
                    return ForwardedSubscriptionRequest.ReadBody(stream);
                case MessageType.NotificationRequest:
                    return NotificationRequest.ReadBody(stream);
                case MessageType.SubscriptionRequest:
                    return SubscriptionRequest.ReadBody(stream);
                case MessageType.MonitorRequest:
                    return MonitorRequest.ReadBody(stream);
                default:
                    throw new InvalidDataException("unknown message type");
            }
        }

        private static MessageType ReadHeader(Stream stream)
        {
            var b = stream.ReadByte();
            if (b == -1)
                throw new EndOfStreamException();
            return (MessageType)b;
        }

        public virtual Stream Write(Stream stream)
        {
            stream.Write((byte)MessageType);
            return stream;
        }

        public override string ToString()
        {
            return $"MessageType={MessageType}";
        }
    }
}
