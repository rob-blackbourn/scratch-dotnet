using System.Collections.Generic;
using JetBlack.MessageBus.FeedBus.Distributor.Interactors;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Publishers
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
