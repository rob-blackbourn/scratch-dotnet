using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public abstract class Message
    {
        public readonly MessageType MessageType;

        protected Message(MessageType messageType)
        {
            MessageType = messageType;
        }

        public static Message Read(Stream stream)
        {
            var messageType = ReadHeader(stream);

            switch (messageType)
            {
                case MessageType.InteractorAdvertisement:
                    return InteractorAdvertisement.ReadBody(stream);
                case MessageType.AuthorizationRequest:
                    return AuthorizationRequest.ReadBody(stream);
                case MessageType.AuthorizationResponse:
                    return AuthorizationResponse.ReadBody(stream);
                case MessageType.MulticastData:
                    return MulticastData.ReadBody(stream);
                case MessageType.UnicastData:
                    return UnicastData.ReadBody(stream);
                case MessageType.ForwardedMulticastData:
                    return ForwardedMulticastData.ReadBody(stream);
                case MessageType.ForwardedUnicastData:
                    return ForwardedUnicastData.ReadBody(stream);
                case MessageType.ForwardedSubscriptionRequest:
                    return ForwardedSubscriptionRequest.ReadBody(stream);
                case MessageType.NotificationRequest:
                    return NotificationRequest.ReadBody(stream);
                case MessageType.SubscriptionRequest:
                    return SubscriptionRequest.ReadBody(stream);
                default:
                    throw new InvalidDataException("unknown message type");
            }
        }

        private static MessageType ReadHeader(Stream stream)
        {
            var b = stream.ReadByte();
            if (b == -1)
                throw new EndOfStreamException();
            var messageType = (MessageType)b;
            return messageType;
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
