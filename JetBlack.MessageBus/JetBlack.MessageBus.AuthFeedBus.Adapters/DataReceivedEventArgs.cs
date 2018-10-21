using System;
using System.Net;
using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(string user, IPAddress address, string feed, string topic, DataPacket[] data, bool isImage)
        {
            User = user;
            Address = address;
            Feed = feed;
            Topic = topic;
            IsImage = isImage; Data = data;
        }

        public string User { get; }
        public IPAddress Address { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public DataPacket[] Data { get; }
    }
}
