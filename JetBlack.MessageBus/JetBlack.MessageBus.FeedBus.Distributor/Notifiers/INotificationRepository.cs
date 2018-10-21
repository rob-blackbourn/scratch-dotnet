using System.Collections.Generic;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;

namespace JetBlack.MessageBus.FeedBus.Distributor.Notifiers
{
    public interface INotificationRepository
    {
        void RemoveInteractor(IInteractor interactor);

        bool AddRequest(IInteractor notifiable, string feed);

        void RemoveRequest(IInteractor notifiable, string feed);

        ISet<IInteractor> FindNotifiables(string feed);
    }
}