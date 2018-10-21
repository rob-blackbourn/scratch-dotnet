using System;
using System.IO;
using System.Net;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class AuthorizationRequest : Message
    {
        public AuthorizationRequest(Guid clientId, IPAddress address, string user, string feed, string topic)
            : base(MessageType.AuthorizationRequest)
        {
            ClientId = clientId;
            Address = address;
            User = user;
            Feed = feed;
            Topic = topic;
        }

        public Guid ClientId { get; }
        public IPAddress Address { get; }
        public string User { get; }
        public string Feed { get; }
        public string Topic { get; }

        public static AuthorizationRequest ReadBody(Stream stream)
        {
            var clientId = stream.ReadGuid();
            var address = stream.ReadIPAddress();
            var user = stream.ReadString();
            var feed = stream.ReadString();
            var topic = stream.ReadString();
            return new AuthorizationRequest(clientId, address, user, feed, topic);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(ClientId);
            stream.Write(Address);
            stream.Write(User);
            stream.Write(Feed);
            stream.Write(Topic);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, ClientId={ClientId}, Address={Address}, User={User}, Feed={Feed}, Topic={Topic}";
        }
    }
}
