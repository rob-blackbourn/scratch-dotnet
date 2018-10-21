namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public class InteractorConnectedEventArgs : InteractorEventArgs
    {
        public InteractorConnectedEventArgs(IInteractor interactor)
            : base(interactor)
        {
        }
    }
}
