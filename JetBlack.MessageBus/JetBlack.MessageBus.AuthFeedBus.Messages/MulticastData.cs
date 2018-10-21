using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class MulticastData : Message
    {
        public MulticastData(string feed, string topic, bool isImage, BinaryDataPacket[] data)
            : base(MessageType.MulticastData)
        {
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
        }

        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public BinaryDataPacket[] Data { get; }

        public static MulticastData ReadBody(Stream stream)
        {
            var feed = stream.ReadString();
            var topic = stream.ReadString();
            var isImage = stream.ReadBoolean();
            var data = stream.ReadBinaryDataPacketArray();
            return new MulticastData(feed, topic, isImage, data);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(Feed);
            stream.Write(Topic);
            stream.Write(IsImage);
            stream.Write(Data);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Feed={Feed}, Topic={Topic}, IsImage={IsImage}, Data={Data?.Length}";
        }
    }
}
