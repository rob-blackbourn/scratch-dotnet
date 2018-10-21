using System;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class BinaryDataPacket
    {
        public BinaryDataPacket(Guid header, byte[] body)
        {
            Header = header;
            Body = body;
        }

        public Guid Header { get; }
        public byte[] Body { get; }
    }
}