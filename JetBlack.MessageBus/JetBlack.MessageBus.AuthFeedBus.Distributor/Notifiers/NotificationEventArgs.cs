using System;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Notifiers
{
    public class NotificationEventArgs : InteractorEventArgs
    {
        public NotificationEventArgs(IInteractor interactor, string feed)
            : base(interactor)
        {
            Feed = feed;
        }

        public string Feed { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, Feed={Feed}";
        }
    }
}
