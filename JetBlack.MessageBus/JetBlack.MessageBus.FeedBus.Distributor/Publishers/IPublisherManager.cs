using System;
using System.Collections.Generic;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Publishers
{
    public interface IPublisherManager
    {
        event EventHandler<StalePublisherEventArgs> StalePublishers;

        void SendUnicastData(IInteractor publisher, UnicastData unicastData, IInteractor subscriber);

        void SendMulticastData(IInteractor publisher, IEnumerable<IInteractor> subscribers, MulticastData multicastData);
    }
}