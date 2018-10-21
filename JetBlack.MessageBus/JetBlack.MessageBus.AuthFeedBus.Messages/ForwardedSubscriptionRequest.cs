using System;
using System.IO;
using System.Net;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class ForwardedSubscriptionRequest : Message
    {
        public ForwardedSubscriptionRequest(string user, IPAddress address, Guid clientId, string feed, string topic, bool isAdd)
            : base(MessageType.ForwardedSubscriptionRequest)
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
        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsAdd { get; }

        public static ForwardedSubscriptionRequest ReadBody(Stream stream)
        {
            var user = stream.ReadString();
            var address = stream.ReadIPAddress();
            var clientId = stream.ReadGuid();
            var feed = stream.ReadString();
            var topic = stream.ReadString();
            var isAdd = stream.ReadBoolean();
            return new ForwardedSubscriptionRequest(user, address, clientId, feed, topic, isAdd);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(User);
            stream.Write(Address);
            stream.Write(ClientId);
            stream.Write(Feed);
            stream.Write(Topic);
            stream.Write(IsAdd);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, User={User}, Address={Address}, ClientId={ClientId}, Feed={Feed}, Topic={Topic}, IsAdd{IsAdd}";
        }
    }
}
