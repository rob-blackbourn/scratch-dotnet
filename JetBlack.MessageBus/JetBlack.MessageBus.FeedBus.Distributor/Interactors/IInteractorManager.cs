using System;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public interface IInteractorManager : IDisposable
    {
        event EventHandler<InteractorClosedEventArgs> ClosedInteractors;
        event EventHandler<InteractorFaultedEventArgs> FaultedInteractors;

        void AddInteractor(IInteractor interactor);

        void CloseInteractor(IInteractor interactor);

        void FaultInteractor(IInteractor interactor, Exception error);
    }
}