using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    internal static class StreamExtensions
    {
        public static void Write(this Stream stream, BinaryDataPacket dataPacket)
        {
            stream.Write(dataPacket.Header);
            stream.Write(dataPacket.Body);
        }

        public static void Write(this Stream stream, BinaryDataPacket[] data)
        {
            if (data == null)
                stream.Write(0);
            else
            {
                stream.Write(data.Length);
                foreach (var dataPacket in data)
                    stream.Write(dataPacket);
            }
        }

        public static BinaryDataPacket ReadBinaryDataPacket(this Stream stream)
        {
            var header = stream.ReadGuid();
            var body = stream.ReadByteArray();
            return new BinaryDataPacket(header, body);
        }

        public static BinaryDataPacket[] ReadBinaryDataPacketArray(this Stream stream)
        {
            var count = stream.ReadInt32();
            if (count == 0)
                return null;

            var data = new BinaryDataPacket[count];
            for (var i = 0; i < count; ++i)
                data[i] = stream.ReadBinaryDataPacket();
            return data;
        }
    }
}
