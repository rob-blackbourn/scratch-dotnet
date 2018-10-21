using System;
using System.IO;
using System.Net;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class ForwardedUnicastData : Message
    {
        public ForwardedUnicastData(string user, IPAddress address, Guid clientId, string feed, string topic, bool isImage, BinaryDataPacket[] data)
            : base(MessageType.ForwardedUnicastData)
        {
            User = user;
            Address = address;
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
        }

        public string User { get; }
        public IPAddress Address { get; }
        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public BinaryDataPacket[] Data { get; }

        public static ForwardedUnicastData ReadBody(Stream stream)
        {
            var user = stream.ReadString();
            var address = stream.ReadIPAddress();
            var clientId = stream.ReadGuid();
            var feed = stream.ReadString();
            var topic = stream.ReadString();
            var isImage = stream.ReadBoolean();
            var data = stream.ReadBinaryDataPacketArray();
            return new ForwardedUnicastData(user, address, clientId, feed, topic, isImage, data);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(User);
            stream.Write(Address);
            stream.Write(ClientId);
            stream.Write(Feed);
            stream.Write(Topic);
            stream.Write(IsImage);
            stream.Write(Data);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, User={User}, Address={Address}, ClientId={ClientId}, Feed={Feed}, Topic={Topic}, IsImage={IsImage}, Data={Data?.Length}";
        }
    }
}
