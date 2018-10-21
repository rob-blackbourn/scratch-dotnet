using System.Collections.Generic;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors;
using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Publishers
{
    public class StalePublisherEventArgs : InteractorEventArgs
    {
        public StalePublisherEventArgs(IInteractor interactor, IList<FeedTopic> feedsAndTopics) 
            : base(interactor)
        {
            FeedsAndTopics = feedsAndTopics;
        }

        public IList<FeedTopic> FeedsAndTopics { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, FeedsAndTopics=[{string.Join(",", FeedsAndTopics)}]";
        }
    }
}
