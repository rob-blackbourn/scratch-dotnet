using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Messages;
using log4net;

namespace JetBlack.MessageBus.FeedBus.Distributor.Publishers
{
    public class PublisherManager : IPublisherManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IPublisherRepository _repository;

        public PublisherManager(IInteractorManager interactorManager)
        {
            _repository = new PublisherRepository();
            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;
        }

        public event EventHandler<StalePublisherEventArgs> StalePublishers;

        public void SendUnicastData(IInteractor publisher, UnicastData unicastData, IInteractor subscriber)
        {
            _repository.AddPublisher(publisher, unicastData.Feed, unicastData.Topic);
            subscriber.SendMessage(unicastData);
        }

        public void SendMulticastData(IInteractor publisher, IEnumerable<IInteractor> subscribers, MulticastData multicastData)
        {
            foreach (var subscriber in subscribers)
                SendMulticastData(publisher, subscriber, multicastData);
        }

        private void SendMulticastData(IInteractor publisher, IInteractor subscriber, MulticastData multicastData)
        {
            if (publisher != null)
                _repository.AddPublisher(publisher, multicastData.Feed, multicastData.Topic);
            subscriber.SendMessage(multicastData);
        }

        private void OnClosedInteractor(object sender, InteractorClosedEventArgs args)
        {
            CloseInteractor(args.Interactor);
        }

        private void OnFaultedInteractor(object sender, InteractorFaultedEventArgs args)
        {
            Log.Debug($"Interactor faulted: {args.Interactor} - {args.Error.Message}");
            CloseInteractor(args.Interactor);
        }

        private void CloseInteractor(IInteractor interactor)
        {
            var topicsWithoutPublishers = _repository.RemovePublisher(interactor).ToList();
            if (topicsWithoutPublishers.Count > 0)
                StalePublishers?.Invoke(this, new StalePublisherEventArgs(interactor, topicsWithoutPublishers));
        }
    }
}
