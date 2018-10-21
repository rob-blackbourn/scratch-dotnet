using System;

namespace JetBlack.MessageBus.FeedBus.Adapters
{
    public interface IClient : IDisposable
    {
        event EventHandler<DataReceivedEventArgs> OnDataReceived;
        event EventHandler<DataErrorEventArgs> OnDataError;
        event EventHandler<ForwardedSubscriptionEventArgs> OnForwardedSubscription;
        event EventHandler<ConnectionChangedEventArgs> OnConnectionChanged;
        event EventHandler<EventArgs> OnHeartbeat;

        void AddSubscription(string feed, string topic);
        void RemoveSubscription(string feed, string topic);
        void AddMonitor(string feed);
        void RemoveMonitor(string feed);
        void AddNotification(string feed);
        void RemoveNotification(string feed);
        void Send(Guid clientId, string feed, string topic, bool isImage, object data);
        void Publish(string feed, string topic, bool isImage, object data);
    }
}
