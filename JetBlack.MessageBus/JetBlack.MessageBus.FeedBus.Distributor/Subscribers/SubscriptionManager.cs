using System.Linq;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Distributor.Notifiers;
using JetBlack.MessageBus.FeedBus.Distributor.Publishers;
using JetBlack.MessageBus.FeedBus.Messages;
using log4net;

namespace JetBlack.MessageBus.FeedBus.Distributor.Subscribers
{
    internal class SubscriptionManager : ISubscriptionManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISubscriptionRepository _repository;
        private readonly INotificationManager _notificationManager;
        private readonly IPublisherManager _publisherManager;

        public SubscriptionManager(IInteractorManager interactorManager, INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;

            _repository = new SubscriptionRepository();
            _publisherManager = new PublisherManager(interactorManager);

            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;

            notificationManager.NewNotificationRequests += OnNewNotificationRequest;

            _publisherManager.StalePublishers += OnStalePublishers;
        }

        public void RequestSubscription(IInteractor subscriber, SubscriptionRequest subscriptionRequest)
        {
            Log.Info($"Received subscription from {subscriber} on \"{subscriptionRequest}\"");

            if (subscriptionRequest.IsAdd)
                _repository.AddSubscription(subscriber, subscriptionRequest.Feed, subscriptionRequest.Topic);
            else
                _repository.RemoveSubscription(subscriber, subscriptionRequest.Feed, subscriptionRequest.Topic, false);

            _notificationManager.ForwardSubscription(new ForwardedSubscriptionRequest(subscriber.Id, subscriptionRequest.Feed, subscriptionRequest.Topic, subscriptionRequest.IsAdd));
        }

        public void RequestMonitor(IInteractor monitor, MonitorRequest monitorRequest)
        {
            Log.Info($"Received monitor from {monitor} on \"{monitorRequest}\"");

            if (monitorRequest.IsAdd)
                _repository.AddMonitor(monitor, monitorRequest.Feed);
            else
                _repository.RemoveMonitor(monitor, monitorRequest.Feed, false);
        }

        private void OnFaultedInteractor(object sender, InteractorFaultedEventArgs args)
        {
            Log.Debug($"Interactor faulted: {args.Interactor} - {args.Error.Message}");

            CloseInteractor(args.Interactor);
        }

        private void OnClosedInteractor(object sender, InteractorClosedEventArgs args)
        {
            CloseInteractor(args.Interactor);
        }

        private void CloseInteractor(IInteractor interactor)
        {
            Log.Debug($"Removing subscriptions for {interactor}");

            // Remove the subscriptions
            var feedTopics = _repository.FindFeedTopicsBySubscriber(interactor).ToList();
            foreach (var feedTopic in feedTopics)
                _repository.RemoveSubscription(interactor, feedTopic.Feed, feedTopic.Topic, true);

            foreach (var feed in feedTopics.Select(x => x.Feed).Distinct())
                _repository.RemoveMonitor(interactor, feed, true);

            // Inform those interested that this interactor is no longer subscribed to these topics.
            foreach (var subscriptionRequest in feedTopics.Select(feedTopic => new ForwardedSubscriptionRequest(interactor.Id, feedTopic.Feed, feedTopic.Topic, false)))
                _notificationManager.ForwardSubscription(subscriptionRequest);
        }

        public void SendUnicastData(IInteractor publisher, UnicastData unicastData)
        {
            // Can we find this client in the subscribers to this topic?
            var subscriber = _repository.GetSubscribersToFeedAndTopic(unicastData.Feed, unicastData.Topic)
                .FirstOrDefault(x => x.Id == unicastData.ClientId);
            if (subscriber == null)
                return;

            _publisherManager.SendUnicastData(publisher, unicastData, subscriber);
        }

        public void SendMulticastData(IInteractor publisher, MulticastData multicastData)
        {
            _publisherManager.SendMulticastData(
                publisher,
                _repository.GetSubscribersToFeedAndTopic(multicastData.Feed, multicastData.Topic),
                multicastData);
        }

        public void OnNewNotificationRequest(object sender, NotificationEventArgs args)
        {
            // Find the subscribers whoes subscriptions match the pattern.
            foreach (var matchingSubscriptions in _repository.GetSubscribersToFeed(args.Feed))
            {
                // Tell the requestor about subscribers that are interested in this topic.
                foreach (var message in matchingSubscriptions.Value.Select(subscriber => new ForwardedSubscriptionRequest(subscriber.Id, args.Feed, matchingSubscriptions.Key, true)))
                    args.Interactor.SendMessage(message);
            }
        }

        public void OnStalePublishers(object sender, StalePublisherEventArgs args)
        {
            foreach (var staleFeedTopic in args.FeedsAndTopics)
                OnStaleTopic(staleFeedTopic);
        }

        private void OnStaleTopic(FeedTopic staleFeedTopic)
        {
            var staleMessage = new MulticastData(staleFeedTopic.Feed, staleFeedTopic.Topic, true, null);

            foreach (var subscriber in _repository.GetSubscribersToFeedAndTopic(staleFeedTopic.Feed, staleFeedTopic.Topic))
                subscriber.SendMessage(staleMessage);
        }
    }
}
