using System;
using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public class AuthorizationResponseEventArg : EventArgs
    {
        public AuthorizationResponseEventArg(IInteractor authorizer, IInteractor requester, AuthorizationResponse response)
        {
            Authorizer = authorizer;
            Requester = requester;
            Response = response;
        }

        public IInteractor Authorizer { get; }
        public IInteractor Requester { get; }
        public AuthorizationResponse Response { get; }
    }
}
