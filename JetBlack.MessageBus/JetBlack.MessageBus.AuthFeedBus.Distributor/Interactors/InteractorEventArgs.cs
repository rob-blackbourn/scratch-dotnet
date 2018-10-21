﻿using System;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
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
