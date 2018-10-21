using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Common.Linq;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Notifiers
{
    public class NotificationRepository
    {
        private readonly Dictionary<string, ISet<IInteractor>> _feedToNotifiables = new Dictionary<string, ISet<IInteractor>>();

        public NotificationRepository()
        {
        }

        public void RemoveInteractor(IInteractor interactor)
        {
            // Remove the interactor where it appears in the notifiables, remembering any topics which are left without any interactors.
            var topicsWithoutInteractors = new HashSet<string>();
            foreach (var topicPatternToNotifiable in _feedToNotifiables.Where(x => x.Value.Contains(interactor)))
            {
                topicPatternToNotifiable.Value.Remove(interactor);
                if (topicPatternToNotifiable.Value.Count == 0)
                    topicsWithoutInteractors.Add(topicPatternToNotifiable.Key);
            }

            // Remove any topics left without interactors.
            foreach (var topic in topicsWithoutInteractors)
            {
                _feedToNotifiables.Remove(topic);
            }
        }

        public bool AddRequest(IInteractor notifiable, string feed)
        {
            // Find or create the set of notifiables for this feed.
            ISet<IInteractor> notifiables;
            if (!_feedToNotifiables.TryGetValue(feed, out notifiables))
            {
                _feedToNotifiables.Add(feed, notifiables = new HashSet<IInteractor>());
            }
            else if (notifiables.Contains(notifiable))
                return false;

            // Add to the notifiables for this topic pattern and inform the subscription manager of the new notification request.
            notifiables.Add(notifiable);
            return true;
        }

        public void RemoveRequest(IInteractor notifiable, string feed)
        {
            // Does this feed have any notifiable interactors?
            ISet<IInteractor> notifiables;
            if (!_feedToNotifiables.TryGetValue(feed, out notifiables))
                return;

            // Is this interactor in the set of notifiables for this feed?
            if (!notifiables.Contains(notifiable))
                return;

            // Remove the interactor from the set of notifiables.
            notifiables.Remove(notifiable);

            // Are there any interactors left listening to this feed?
            if (notifiables.Count != 0)
                return;

            // Remove the empty pattern from the caches.
            _feedToNotifiables.Remove(feed);
        }

        public ISet<IInteractor> FindNotifiables(string feed)
        {
            ISet<IInteractor> interactors;
            return !_feedToNotifiables.TryGetValue(feed, out interactors) ? null : interactors;
        }
    }
}
