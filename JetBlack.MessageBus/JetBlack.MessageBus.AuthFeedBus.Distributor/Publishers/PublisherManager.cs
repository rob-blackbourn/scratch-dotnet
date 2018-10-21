using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;
using JetBlack.MessageBus.AuthFeedBus.Messages;
using log4net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Publishers
{
    public class PublisherManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PublisherRepository _repository;

        public PublisherManager(InteractorManager interactorManager)
        {
            _repository = new PublisherRepository();
            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;
        }

        public EventHandler<StalePublisherEventArgs> StalePublishers;

        public void SendUnicastData(IInteractor publisher, IInteractor subscriber, AuthorizationInfo authorization, UnicastData unicastData)
        {
            if (!publisher.HasRole(unicastData.Feed, Role.Publish))
            {
                Log.Warn($"Rejected request from {publisher} to publish on feed {unicastData.Feed}");
                return;
            }

            var clientUnicastData =
                authorization.IsAuthorizationRequired
                    ? new ForwardedUnicastData(publisher.User, publisher.Address, unicastData.ClientId, unicastData.Feed, unicastData.Topic, unicastData.IsImage, unicastData.Data.Where(x => authorization.Entitlements.Contains(x.Header)).ToArray())
                    : new ForwardedUnicastData(publisher.User, publisher.Address, unicastData.ClientId, unicastData.Feed, unicastData.Topic, unicastData.IsImage, unicastData.Data);

            Log.Debug($"Sending unicast data from {publisher} to {subscriber}: {clientUnicastData}");

            _repository.AddPublisher(publisher, clientUnicastData.Feed, clientUnicastData.Topic);

            try
            {
                subscriber.SendMessage(clientUnicastData);
            }
            catch (Exception exception)
            {
                Log.Debug($"Failed to send to subscriber {subscriber} unicast data {clientUnicastData}", exception);
            }
        }

        public void SendMulticastData(IInteractor publisher, IEnumerable<KeyValuePair<IInteractor, AuthorizationInfo>> subscribers, MulticastData multicastData)
        {
            if (!(publisher == null || publisher.HasRole(multicastData.Feed, Role.Publish)))
            {
                Log.Warn($"Rejected request from {publisher} to publish to Feed {multicastData.Feed}");
                return;
            }

            foreach (var subscriberAndAuthorizationInfo in subscribers)
            {
                var subscriber = subscriberAndAuthorizationInfo.Key;
                var authorizationInfo = subscriberAndAuthorizationInfo.Value;

                var subscriberMulticastData =
                    subscriberAndAuthorizationInfo.Value.IsAuthorizationRequired
                        ? new ForwardedMulticastData(publisher?.User ?? "internal", publisher?.Address ?? IPAddress.None, multicastData.Feed, multicastData.Topic, multicastData.IsImage, FilterDataPackets(authorizationInfo.Entitlements, multicastData.Data))
                        : new ForwardedMulticastData(publisher?.User ?? "internal", publisher?.Address ?? IPAddress.None, multicastData.Feed, multicastData.Topic, multicastData.IsImage, multicastData.Data);

                Log.Debug($"Sending multicast data from {publisher} to {subscriber}: {subscriberMulticastData}");

                if (publisher != null)
                    _repository.AddPublisher(publisher, subscriberMulticastData.Feed, subscriberMulticastData.Topic);

                try
                {
                    subscriber.SendMessage(subscriberMulticastData);
                }
                catch (Exception exception)
                {
                    Log.Debug($"Failed to send to subscriber {subscriber} multicast data {subscriberMulticastData}", exception);
                }
            }
        }

        private BinaryDataPacket[] FilterDataPackets(ISet<Guid> authorizations, BinaryDataPacket[] data)
        {
            return data.Where(x => authorizations.Contains(x.Header)).ToArray();
        }

        public void OnClosedInteractor(object sender, InteractorClosedEventArgs args)
        {
            CloseInteractor(args.Interactor);
        }

        public void OnFaultedInteractor(object sender, InteractorFaultedEventArgs args)
        {
            Log.Debug($"Interactor faulted: {args.Interactor}", args.Error);
            CloseInteractor(args.Interactor);
        }

        private void CloseInteractor(IInteractor interactor)
        {
            var topicsWithoutPublishers = _repository.RemovePublisher(interactor);
            if (topicsWithoutPublishers != null)
                StalePublishers?.Invoke(this, new StalePublisherEventArgs(interactor, topicsWithoutPublishers.ToList()));
        }
    }
}
