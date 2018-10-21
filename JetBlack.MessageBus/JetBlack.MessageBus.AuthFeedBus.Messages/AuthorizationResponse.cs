using System;
using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class AuthorizationResponse : Message
    {
        public AuthorizationResponse(Guid clientId, string feed, string topic, bool isAuthorizationRequired, Guid[] entitlements)
            : base(MessageType.AuthorizationResponse)
        {
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAuthorizationRequired = isAuthorizationRequired;
            Entitlements = entitlements;
        }

        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsAuthorizationRequired { get; }
        public Guid[] Entitlements { get; }

        public static AuthorizationResponse ReadBody(Stream stream)
        {
            var clientId = stream.ReadGuid();
            var feed = stream.ReadString();
            var topic = stream.ReadString();
            var isAuthorizationRequired = stream.ReadBoolean();
            var entitlements = stream.ReadGuidArray();
            return new AuthorizationResponse(clientId, feed, topic, isAuthorizationRequired, entitlements);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(ClientId);
            stream.Write(Feed);
            stream.Write(Topic);
            stream.Write(IsAuthorizationRequired);
            stream.Write(Entitlements);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, ClientId={ClientId}, Feed={Feed}, Topic={Topic}, IsAuthorizationRequired={IsAuthorizationRequired}, Entitlements={Entitlements?.Length}";
        }
    }
}
