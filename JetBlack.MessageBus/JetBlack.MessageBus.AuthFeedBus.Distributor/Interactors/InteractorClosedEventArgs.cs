using System;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public class InteractorClosedEventArgs : EventArgs
    {
        public InteractorClosedEventArgs(IInteractor interactor)
        {
            Interactor = interactor;
        }

        public IInteractor Interactor { get; }
    }
}
