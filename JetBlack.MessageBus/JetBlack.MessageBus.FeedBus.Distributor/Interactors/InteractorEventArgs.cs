using System;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public class InteractorEventArgs : EventArgs
    {
        public InteractorEventArgs(IInteractor interactor)
        {
            Interactor = interactor;
        }

        public IInteractor Interactor { get; }

        public override string ToString()
        {
            return $"Interactor={Interactor}";
        }
    }
}
