namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public class InteractorConnectedEventArgs : InteractorEventArgs
    {
        public InteractorConnectedEventArgs(IInteractor interactor)
            : base(interactor)
        {
        }
    }
}
