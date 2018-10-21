using System;
using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class UnicastData : Message
    {
        public UnicastData(Guid clientId, string feed, string topic, bool isImage, BinaryDataPacket[] data)
            : base(MessageType.UnicastData)
        {
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
        }

        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public BinaryDataPacket[] Data { get; }

        public static UnicastData ReadBody(Stream stream)
        {
            var clientId = stream.ReadGuid();
            var feed = stream.ReadString();
            var topic = stream.ReadString();
            var isImage = stream.ReadBoolean();
            var data = stream.ReadBinaryDataPacketArray();
            return new UnicastData(clientId, feed, topic, isImage, data);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(ClientId);
            stream.Write(Feed);
            stream.Write(Topic);
            stream.Write(IsImage);
            stream.Write(Data);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, ClientId={ClientId}, Feed={Feed}, Topic={Topic}, IsImage={IsImage}, Data={Data?.Length}";
        }
    }
}
