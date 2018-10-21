using System.Collections.Generic;
using JetBlack.MessageBus.Common.Collections.Generic;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors;
using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Publishers
{
    public class PublisherRepository
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
            return _topicsAndPublishers.Remove(publisher);
        }
    }
}
