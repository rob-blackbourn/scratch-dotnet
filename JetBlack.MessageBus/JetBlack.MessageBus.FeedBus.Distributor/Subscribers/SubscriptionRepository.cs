using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Subscribers
{
    internal class SubscriptionRepository : ISubscriptionRepository
    {
        // Feed->Topic->Interactor->SubscriptionCount.
        private readonly IDictionary<string, IDictionary<string, IDictionary<IInteractor, SubscriptionState>>> _subscriptions = new Dictionary<string, IDictionary<string, IDictionary<IInteractor, SubscriptionState>>>();
        // Feed->Topic->Interactor->SubscriptionCount.
        private readonly IDictionary<string, IDictionary<IInteractor, SubscriptionState>> _monitors = new Dictionary<string, IDictionary<IInteractor, SubscriptionState>>();

        public SubscriptionRepository()
        {
        }

        public void AddSubscription(IInteractor subscriber, string feed, string topic)
        {
            // Find topic subscriptions for this feed.
            IDictionary<string, IDictionary<IInteractor, SubscriptionState>> topicSubscriptions;
            if (!_subscriptions.TryGetValue(feed, out topicSubscriptions))
                _subscriptions.Add(feed, topicSubscriptions = new Dictionary<string, IDictionary<IInteractor, SubscriptionState>>());

            // Find the list of interactors that have subscribed to this topic.
            IDictionary<IInteractor, SubscriptionState> subscribersForTopic;
            if (!topicSubscriptions.TryGetValue(topic, out subscribersForTopic))
                topicSubscriptions.Add(topic, subscribersForTopic = new Dictionary<IInteractor, SubscriptionState>());

            // Find this interactor.
            SubscriptionState subscriptionState;
            if (!subscribersForTopic.TryGetValue(subscriber, out subscriptionState))
                subscribersForTopic.Add(subscriber, subscriptionState = new SubscriptionState());

            // Increment the subscription count.
            subscriptionState.Count = subscriptionState.Count + 1;
        }

        public void RemoveSubscription(IInteractor subscriber, string feed, string topic, bool removeAll)
        {
            // Can we find topic subscriptions this feed?
            IDictionary<string, IDictionary<IInteractor, SubscriptionState>> topicSubscriptions;
            if (!_subscriptions.TryGetValue(feed, out topicSubscriptions))
                return;

            // Can we find subscribers for this topic?
            IDictionary<IInteractor, SubscriptionState> subscribersForTopic;
            if (!topicSubscriptions.TryGetValue(topic, out subscribersForTopic))
                return;

            // Has this subscriber registered an interest in the topic?
            SubscriptionState subscriptionState;
            if (!subscribersForTopic.TryGetValue(subscriber, out subscriptionState))
                return;

            if (removeAll || --subscriptionState.Count == 0)
                subscribersForTopic.Remove(subscriber);

            // If there are no subscribers left on this topic, remove it from the feed.
            if (subscribersForTopic.Count == 0)
                topicSubscriptions.Remove(topic);

            // If there are no topics left in the feed, remove it from the cache.
            if (topicSubscriptions.Count == 0)
                _subscriptions.Remove(feed);
        }

        public void AddMonitor(IInteractor monitor, string feed)
        {
            // Find monitors to the feed.
            IDictionary<IInteractor, SubscriptionState> feedMonitors;
            if (!_monitors.TryGetValue(feed, out feedMonitors))
                _monitors.Add(feed, feedMonitors = new Dictionary<IInteractor, SubscriptionState>());

            // Find the subscription state of this monitor.
            SubscriptionState subscriptionState;
            if (!feedMonitors.TryGetValue(monitor, out subscriptionState))
                feedMonitors.Add(monitor, subscriptionState = new SubscriptionState());

            // Increment the subscription count.
            subscriptionState.Count = subscriptionState.Count + 1;
        }

        public void RemoveMonitor(IInteractor monitor, string feed, bool removeAll)
        {
            // Can we find monitors for this feed in the cache?
            IDictionary<IInteractor, SubscriptionState> feedMonitors;
            if (!_monitors.TryGetValue(feed, out feedMonitors))
                return;

            // Does this monitor have a subscription state?
            SubscriptionState subscriptionState;
            if (!feedMonitors.TryGetValue(monitor, out subscriptionState))
                return;

            if (removeAll || --subscriptionState.Count == 0)
                feedMonitors.Remove(monitor);

            // If there are no topics left in the feed, remove it from the cache.
            if (feedMonitors.Count == 0)
                _monitors.Remove(feed);
        }

        public IEnumerable<FeedTopic> FindFeedTopicsBySubscriber(IInteractor subscriber)
        {
            return _subscriptions
                .Select(topicCache =>
                    topicCache.Value
                        .Where(x => x.Value.ContainsKey(subscriber))
                        .Select(x => new FeedTopic(topicCache.Key, x.Key)))
                .SelectMany(x => x);
        }

        public IEnumerable<IInteractor> GetSubscribersToFeedAndTopic(string feed, string topic)
        {
            var subscribers = new List<IInteractor>();

            // Look for subscriptions to this feed.
            IDictionary<string, IDictionary<IInteractor, SubscriptionState>> topicSubscriptions;
            if (_subscriptions.TryGetValue(feed, out topicSubscriptions))
            {
                // Are there subscribers for this topic?
                IDictionary<IInteractor, SubscriptionState> subscribersForTopic;
                if (topicSubscriptions.TryGetValue(topic, out subscribersForTopic))
                    subscribers.AddRange(subscribersForTopic.Keys);
            }

            // Look for monitors to this feed.
            IDictionary<IInteractor, SubscriptionState> feedMonitors;
            if (_monitors.TryGetValue(feed, out feedMonitors))
                subscribers.AddRange(feedMonitors.Keys);

            return subscribers;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<IInteractor>>> GetSubscribersToFeed(string feed)
        {
            // Can we find this feed in the cache?
            IDictionary<string, IDictionary<IInteractor, SubscriptionState>> topicCache;
            if (_subscriptions.TryGetValue(feed, out topicCache))
                return topicCache.Select(x => KeyValuePair.Create(x.Key, x.Value.Keys.AsEnumerable()));

            return new KeyValuePair<string, IEnumerable<IInteractor>>[0];
        }

        private class SubscriptionState
        {
            public int Count { get; set; }
        }
    }
}
