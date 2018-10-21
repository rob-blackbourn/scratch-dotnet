using System;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public interface IInteractorRepository : IDisposable
    {
        void Add(IInteractor interactor);

        bool Remove(IInteractor interactor);
    }
}