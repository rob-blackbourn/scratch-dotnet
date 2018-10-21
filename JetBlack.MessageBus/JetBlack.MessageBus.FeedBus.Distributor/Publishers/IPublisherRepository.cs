using System.Collections.Generic;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Publishers
{
    public interface IPublisherRepository
    {
        void AddPublisher(IInteractor publisher, string feed, string topic);

        IEnumerable<FeedTopic> RemovePublisher(IInteractor publisher);
    }
}