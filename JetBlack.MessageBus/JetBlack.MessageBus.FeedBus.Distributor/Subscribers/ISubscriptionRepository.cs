using System.Collections.Generic;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Subscribers
{
    public interface ISubscriptionRepository
    {
        void AddSubscription(IInteractor subscriber, string feed, string topic);

        void RemoveSubscription(IInteractor subscriber, string feed, string topic, bool removeAll);

        void AddMonitor(IInteractor monitor, string feed);

        void RemoveMonitor(IInteractor monitor, string feed, bool removeAll);

        IEnumerable<FeedTopic> FindFeedTopicsBySubscriber(IInteractor subscriber);

        IEnumerable<IInteractor> GetSubscribersToFeedAndTopic(string feed, string topic);

        IEnumerable<KeyValuePair<string, IEnumerable<IInteractor>>> GetSubscribersToFeed(string feed);
    }
}