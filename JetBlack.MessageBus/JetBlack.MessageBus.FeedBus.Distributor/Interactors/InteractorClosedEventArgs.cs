using System;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
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
