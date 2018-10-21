using System;
using System.Collections.Generic;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;
using JetBlack.MessageBus.AuthFeedBus.Messages;
using log4net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public class InteractorManager : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly InteractorRepository _repository;

        internal InteractorManager(DistributorRole distributorRole)
        {
            _repository = new InteractorRepository(distributorRole);
        }

        internal EventHandler<AuthorizationResponseEventArg> AuthorizationResponses;
        internal EventHandler<InteractorClosedEventArgs> ClosedInteractors;
        internal EventHandler<InteractorFaultedEventArgs> FaultedInteractors;

        internal void OpenInteractor(IInteractor interactor)
        {
            Log.Debug($"Opening interactor {interactor}.");

            var joinMessage = new InteractorAdvertisement(interactor.User, interactor.Address, true);
            var existingJoinMessages = new List<InteractorAdvertisement>();
            foreach (var existingInteractor in _repository)
            {
                existingInteractor.SendMessage(joinMessage);
                existingJoinMessages.Add(new InteractorAdvertisement(existingInteractor.User, existingInteractor.Address, true));
            }

            _repository.Add(interactor);

            interactor.Start();

            foreach (var message in existingJoinMessages)
                interactor.SendMessage(message);
        }

        internal void CloseInteractor(IInteractor interactor)
        {
            Log.Debug($"Closing interactor {interactor}.");
            RemoveInteractor(interactor);
            ClosedInteractors?.Invoke(this, new InteractorClosedEventArgs(interactor));
        }

        internal void FaultInteractor(IInteractor interactor, Exception error)
        {
            Log.Debug($"Faulting interactor {interactor}.");
            RemoveInteractor(interactor);
            FaultedInteractors?.Invoke(this, new InteractorFaultedEventArgs(interactor, error));
        }

        internal void AdvertiseInterator(IInteractor interactor, InteractorAdvertisement message)
        {
            foreach (var i in _repository)
                if (i.Id != interactor.Id)
                    i.SendMessage(message);
        }

        private void RemoveInteractor(IInteractor interactor)
        {
            _repository.Remove(interactor);

            var leaveMessage = new InteractorAdvertisement(interactor.User, interactor.Address, false);
            foreach (var existingInteractor in _repository)
                existingInteractor.SendMessage(leaveMessage);
        }

        internal void RequestAuthorisation(IInteractor interactor, string feed, string topic)
        {
            Log.Debug($"Requesting authorisation Interactor={interactor}, Feed={feed}, Topic={topic}");

            if (!IsAuthorizationRequired(feed))
            {
                Log.Debug("No authorisation required");
                AcceptAuthorization(interactor, new AuthorizationResponse(interactor.Id, feed, topic, false, null));
            }
            else
            {
                var authorizationRequest = new AuthorizationRequest(interactor.Id, interactor.Address, interactor.User, feed, topic);

                foreach (var authorizer in _repository.Find(feed, Role.Authorize))
                {
                    try
                    {
                        Log.Debug($"Requesting authorization from {authorizer}");
                        authorizer.SendMessage(authorizationRequest);
                    }
                    catch (Exception exception)
                    {
                        Log.Warn($"Failed to send {authorizer} message {authorizationRequest}", exception);
                    }
                }
            }
        }

        internal void AcceptAuthorization(IInteractor authorizer, AuthorizationResponse message)
        {
            Log.Debug($"Accepting an authorization response from {authorizer} with {message}.");

            var requestor = _repository.Find(message.ClientId);
            if (requestor == null)
            {
                Log.Warn($"Unable to queue an authorization response for unknown ClientId={message.ClientId} for Feed=\"{message.Feed}\", Topic=\"{message.Topic}\".");
                return;
            }

            AuthorizationResponses?.Invoke(this, new AuthorizationResponseEventArg(authorizer, requestor, message));
        }

        private bool IsAuthorizationRequired(string feed)
        {
            // Only specifically configured feeds require authroization.
            return _repository.DistributorRole.FeedRoles.ContainsKey(feed);
        }

        public void Dispose()
        {
            Log.Debug("Disposing");
            _repository.Dispose();
        }
    }
}
