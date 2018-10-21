﻿using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
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