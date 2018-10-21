using System.Collections.Generic;
using JetBlack.MessageBus.Common.Collections.Generic;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Publishers
{
    internal class PublisherRepository : IPublisherRepository
    {
        private readonly TwoWaySet<FeedTopic, IInteractor> _topicsAndPublishers = new TwoWaySet<FeedTopic, IInteractor>();

        public PublisherRepository()
        {
        }

        public void AddPublisher(IInteractor publisher, string feed, string topic)
        {
            _topicsAndPublishers.Add(publisher, new FeedTopic(feed, topic));
        }

        public IEnumerable<FeedTopic> RemovePublisher(IInteractor publisher)
        {
            return _topicsAndPublishers.Remove(publisher) ?? new FeedTopic[0];
        }
    }
}
