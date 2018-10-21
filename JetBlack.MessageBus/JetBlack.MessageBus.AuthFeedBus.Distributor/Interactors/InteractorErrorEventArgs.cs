using System;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public class InteractorErrorEventArgs : InteractorEventArgs
    {
        public InteractorErrorEventArgs(IInteractor interactor, Exception error) 
            : base(interactor)
        {
            Error = error;
        }

        public Exception Error { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, Error={Error?.Message}";
        }
    }
}
