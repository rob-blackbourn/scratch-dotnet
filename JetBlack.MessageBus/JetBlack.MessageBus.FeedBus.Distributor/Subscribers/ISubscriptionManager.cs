using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Distributor.Notifiers;
using JetBlack.MessageBus.FeedBus.Distributor.Publishers;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Subscribers
{
    public interface ISubscriptionManager
    {
        void RequestSubscription(IInteractor subscriber, SubscriptionRequest subscriptionRequest);

        void RequestMonitor(IInteractor monitor, MonitorRequest monitorRequest);

        void SendUnicastData(IInteractor publisher, UnicastData unicastData);

        void SendMulticastData(IInteractor publisher, MulticastData multicastData);

        void OnNewNotificationRequest(object sender, NotificationEventArgs args);

        void OnStalePublishers(object sender, StalePublisherEventArgs args);
    }
}