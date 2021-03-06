﻿using System;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors;
using JetBlack.MessageBus.AuthFeedBus.Messages;
using log4net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Notifiers
{
    public class NotificationManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly NotificationRepository _repository;

        public NotificationManager(InteractorManager interactorManager)
        {
            _repository = new NotificationRepository();
            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;
        }

        public EventHandler<NotificationEventArgs> NewNotificationRequests;

        public void OnFaultedInteractor(object sender, InteractorFaultedEventArgs args)
        {
            Log.Debug($"Interactor faulted: {args.Interactor}", args.Error);
            _repository.RemoveInteractor(args.Interactor);
        }

        public void OnClosedInteractor(object sender, InteractorClosedEventArgs args)
        {
            Log.Debug($"Removing notification requests from {args.Interactor}");
            _repository.RemoveInteractor(args.Interactor);
        }

        public void RequestNotification(IInteractor notifiable, NotificationRequest notificationRequest)
        {
            Log.Debug($"Handling notification request for {notifiable} on {notificationRequest}");

            if (notificationRequest.IsAdd)
            {
                if (_repository.AddRequest(notifiable, notificationRequest.Feed))
                    NewNotificationRequests?.Invoke(this, new NotificationEventArgs(notifiable, notificationRequest.Feed));
            }
            else
                _repository.RemoveRequest(notifiable, notificationRequest.Feed);
        }

        internal void ForwardSubscription(IInteractor subscriber, SubscriptionRequest subscriptionRequest)
        {
            // Find all the interactors that wish to be notified of subscriptions to this topic.
            var notifiables = _repository.FindNotifiables(subscriptionRequest.Feed);
            if (notifiables == null)
                return;

            var forwardedSubscriptionRequest = new ForwardedSubscriptionRequest(subscriber.User, subscriber.Address, subscriber.Id, subscriptionRequest.Feed, subscriptionRequest.Topic, subscriptionRequest.IsAdd);

            Log.Debug($"Notifying interactors[{string.Join(",", notifiables)}] of subscription {forwardedSubscriptionRequest}");

            // Inform each notifiable interactor of the subscription request.
            foreach (var notifiable in notifiables)
            {
                try
                {
                    notifiable.SendMessage(forwardedSubscriptionRequest);
                }
                catch (Exception exception)
                {
                    Log.Debug($"Failed to notify {notifiable} regarding {forwardedSubscriptionRequest}", exception);
                }
            }
        }
    }
}
