namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public interface IInteractorListener
    {
        IInteractor Accept();
    }
}
