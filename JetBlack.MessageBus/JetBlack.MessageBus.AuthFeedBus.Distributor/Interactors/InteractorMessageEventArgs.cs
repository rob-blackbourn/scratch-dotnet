using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public class InteractorMessageEventArgs : InteractorEventArgs
    {
        public InteractorMessageEventArgs(IInteractor interactor, Message message)
            : base(interactor)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}