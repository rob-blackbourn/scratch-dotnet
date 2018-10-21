using System;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class DataPacket
    {
        public DataPacket(Guid header, object body)
        {
            Header = header;
            Body = body;
        }

        public Guid Header { get; }
        public object Body { get; }
    }
}
