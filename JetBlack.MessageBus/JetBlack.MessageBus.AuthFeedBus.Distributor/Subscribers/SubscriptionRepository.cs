using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors;
using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Subscribers
{
    public class SubscriptionRepository
    {
        // Feed->Topic->Interactor->SubscriptionCount.
        private readonly IDictionary<string, IDictionary<string, IDictionary<IInteractor, SubscriptionState>>> _cache = new Dictionary<string, IDictionary<string, IDictionary<IInteractor, SubscriptionState>>>();

        public SubscriptionRepository()
        {
        }

        internal void AddSubscription(IInteractor subscriber, string feed, string topic, AuthorizationInfo authorizationInfo)
        {
            // Find topic subscriptions for this feed.
            IDictionary<string, IDictionary<IInteractor, SubscriptionState>> topicCache;
            if (!_cache.TryGetValue(feed, out topicCache))
                _cache.Add(feed, topicCache = new Dictionary<string, IDictionary<IInteractor, SubscriptionState>>());

            // Find subscribers to this topic.
            IDictionary<IInteractor, SubscriptionState> subscribersForTopic;
            if (!topicCache.TryGetValue(topic, out subscribersForTopic))
                topicCache.Add(topic, subscribersForTopic = new Dictionary<IInteractor, SubscriptionState>());

            // Find the subscription state for this subscriber.
            SubscriptionState subscriptionState;
            if (!subscribersForTopic.TryGetValue(subscriber, out subscriptionState))
                subscribersForTopic.Add(subscriber, subscriptionState = new SubscriptionState(authorizationInfo));

            // Increment the subscription count.
            subscriptionState.Count = subscriptionState.Count + 1;
        }

        internal void RemoveSubscription(IInteractor subscriber, string feed, string topic, bool removeAll)
        {
            // Can we find topic subscriptions for this feed?
            IDictionary<string, IDictionary<IInteractor, SubscriptionState>> topicCache;
            if (!_cache.TryGetValue(feed, out topicCache))
                return;

            // Can we find subscribers for this topic?
            IDictionary<IInteractor, SubscriptionState> subscribersForTopic;
            if (!topicCache.TryGetValue(topic, out subscribersForTopic))
                return;

            // Can we find a subscription state for this subscriber?
            SubscriptionState subscriptionState;
            if (!subscribersForTopic.TryGetValue(subscriber, out subscriptionState))
                return;

            // If we are removing all subscribers, or this subscriber has no active subscriptions, remove the subscriber.
            if (removeAll || --subscriptionState.Count == 0)
                subscribersForTopic.Remove(subscriber);

            // If there are no subscribers left on this topic, remove it from the feed.
            if (subscribersForTopic.Count == 0)
                topicCache.Remove(topic);

            // If there are no topics left in the feed, remove it from the cache.
            if (topicCache.Count == 0)
                _cache.Remove(feed);
        }

        internal IEnumerable<FeedTopic> FindByInteractor(IInteractor interactor)
        {
            return _cache
                .Select(topicCache =>
                    topicCache.Value
                        .Where(x => x.Value.ContainsKey(interactor))
                        .Select(x => new FeedTopic(topicCache.Key, x.Key)))
                .SelectMany(x => x);
        }

        internal IEnumerable<KeyValuePair<IInteractor, AuthorizationInfo>> GetSubscribersToFeedAndTopic(string feed, string topic)
        {
            // Can we find this feed in the cache?
            IDictionary<string, IDictionary<IInteractor, SubscriptionState>> topicCache;
            if (_cache.TryGetValue(feed, out topicCache))
            {
                // Are there subscribers for this topic?
                IDictionary<IInteractor, SubscriptionState> subscribersForTopic;
                if (topicCache.TryGetValue(topic, out subscribersForTopic))
                    return subscribersForTopic.Select(x => KeyValuePair.Create(x.Key, x.Value.AuthorizationState));
            }
            return new KeyValuePair<IInteractor, AuthorizationInfo>[0];
        }

        internal IEnumerable<KeyValuePair<string, IEnumerable<IInteractor>>> GetSubscribersToFeed(string feed)
        {
            // Can we find this feed in the cache?
            IDictionary<string, IDictionary<IInteractor, SubscriptionState>> topicCache;
            if (_cache.TryGetValue(feed, out topicCache))
                return topicCache.Select(x => KeyValuePair.Create(x.Key, x.Value.Keys.AsEnumerable()));

            return new KeyValuePair<string, IEnumerable<IInteractor>>[0];
        }

        private class SubscriptionState
        {
            public SubscriptionState(AuthorizationInfo authorizationInfo)
            {
                AuthorizationState = authorizationInfo;
            }

            public int Count { get; set; }
            public AuthorizationInfo AuthorizationState { get; }
            public ISet<Guid> Entitlements { get; private set; }
        }
    }

}
