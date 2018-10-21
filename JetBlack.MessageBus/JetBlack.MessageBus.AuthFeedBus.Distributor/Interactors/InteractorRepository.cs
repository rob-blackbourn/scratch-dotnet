using System;
using System.Collections;
using System.Collections.Generic;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public class InteractorRepository : IDisposable, IEnumerable<IInteractor>
    {
        private readonly IDictionary<Guid, IInteractor> _interactors = new Dictionary<Guid, IInteractor>();
        private readonly IDictionary<string, IDictionary<Role, ISet<IInteractor>>> _feedRoleInteractors = new Dictionary<string, IDictionary<Role, ISet<IInteractor>>>();

        internal InteractorRepository(DistributorRole distributorRole)
        {
            DistributorRole = distributorRole;
        }

        internal DistributorRole DistributorRole { get; }

        internal void Add(IInteractor interactor)
        {
            _interactors.Add(interactor.Id, interactor);
            AddFeedRoles(interactor);
        }

        internal void Remove(IInteractor interactor)
        {
            RemoveFeedRoles(interactor);
            _interactors.Remove(interactor.Id);
        }

        internal IInteractor Find(Guid id)
        {
            IInteractor requestor;
            if (_interactors.TryGetValue(id, out requestor))
                return requestor;
            return null;
        }

        internal IEnumerable<IInteractor> Find(string feed, Role role)
        {
            IDictionary<Role, ISet<IInteractor>> roleInteractors;
            if (_feedRoleInteractors.TryGetValue(feed, out roleInteractors))
            {
                ISet<IInteractor> interactors;
                if (roleInteractors.TryGetValue(role, out interactors))
                    return interactors;
            }

            return new IInteractor[0];
        }

        private void AddFeedRoles(IInteractor interactor)
        {
            foreach (var feed in DistributorRole.FeedRoles.Keys)
            {
                IDictionary<Role, ISet<IInteractor>> roleInteractor;
                if (!_feedRoleInteractors.TryGetValue(feed, out roleInteractor))
                    _feedRoleInteractors.Add(feed, roleInteractor = new Dictionary<Role, ISet<IInteractor>>());

                foreach (var role in new[] { Role.Publish, Role.Subscribe, Role.Notify, Role.Authorize })
                {
                    if (DistributorRole.HasRole(interactor.Address, interactor.User, feed, role))
                    {
                        ISet<IInteractor> interactors;
                        if (!roleInteractor.TryGetValue(role, out interactors))
                            roleInteractor.Add(role, interactors = new HashSet<IInteractor>());

                        interactors.Add(interactor);
                    }
                }
            }
        }

        private void RemoveFeedRoles(IInteractor interactor)
        {
            var feedsModified = new HashSet<string>();

            foreach (var feedRoleInteractors in _feedRoleInteractors)
            {
                var rolesModified = new HashSet<Role>();

                foreach (var roleInteractors in feedRoleInteractors.Value)
                {
                    if (roleInteractors.Value.Remove(interactor))
                        rolesModified.Add(roleInteractors.Key);
                }

                foreach (var role in rolesModified)
                {
                    if (feedRoleInteractors.Value[role].Count == 0)
                    {
                        feedRoleInteractors.Value.Remove(role);
                        feedsModified.Add(feedRoleInteractors.Key);
                    }
                }
            }

            foreach (var feed in feedsModified)
            {
                if (_feedRoleInteractors[feed].Count == 0)
                    _feedRoleInteractors.Remove(feed);
            }
        }

        public void Dispose()
        {
            foreach (var interactor in _interactors.Values)
                interactor.Dispose();
        }

        public IEnumerator<IInteractor> GetEnumerator()
        {
            return _interactors.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
