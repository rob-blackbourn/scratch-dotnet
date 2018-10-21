using System;

namespace JetBlack.MessageBus.FeedBus.Adapters
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(string feed, string topic, object data, bool isImage)
        {
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
        }

        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public object Data { get; }
    }
}
