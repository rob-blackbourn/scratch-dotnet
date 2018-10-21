using System;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Messages;
using log4net;

namespace JetBlack.MessageBus.FeedBus.Distributor.Notifiers
{
    public interface INotificationManager
    {
        event EventHandler<NotificationEventArgs> NewNotificationRequests;

        void RequestNotification(IInteractor notifiable, NotificationRequest notificationRequest);

        void ForwardSubscription(ForwardedSubscriptionRequest forwardedSubscriptionRequest);
    }

    public class NotificationManager : INotificationManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly INotificationRepository _repository;

        public NotificationManager(IInteractorManager interactorManager)
        {
            _repository = new NotificationRepository();
            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;
        }

        public event EventHandler<NotificationEventArgs> NewNotificationRequests;

        private void OnFaultedInteractor(object sender, InteractorFaultedEventArgs args)
        {
            Log.Debug($"Interactor faulted: {args.Interactor} - {args.Error.Message}");
            RemoveInteractor(args.Interactor);
        }

        private void OnClosedInteractor(object sender, InteractorClosedEventArgs args)
        {
            Log.Debug($"Removing notification requests from {args.Interactor}");
            RemoveInteractor(args.Interactor);
        }

        private void RemoveInteractor(IInteractor interactor)
        {
            _repository.RemoveInteractor(interactor);
        }

        public void RequestNotification(IInteractor notifiable, NotificationRequest notificationRequest)
        {
            Log.Info($"Handling notification request for {notifiable} on {notificationRequest}");

            if (notificationRequest.IsAdd)
            {
                if (_repository.AddRequest(notifiable, notificationRequest.Feed))
                    NewNotificationRequests?.Invoke(this, new NotificationEventArgs(notifiable, notificationRequest.Feed));
            }
            else
                _repository.RemoveRequest(notifiable, notificationRequest.Feed);
        }

        public void ForwardSubscription(ForwardedSubscriptionRequest forwardedSubscriptionRequest)
        {
            // Find all the interactors that wish to be notified of subscriptions to this topic.
            var notifiables = _repository.FindNotifiables(forwardedSubscriptionRequest.Feed);
            if (notifiables == null)
                return;

            Log.Debug($"Notifying interactors[{string.Join(",", notifiables)}] of subscription {forwardedSubscriptionRequest}");

            // Inform each notifiable interactor of the subscription request.
            foreach (var notifiable in notifiables)
                notifiable.SendMessage(forwardedSubscriptionRequest);
        }
    }
}
