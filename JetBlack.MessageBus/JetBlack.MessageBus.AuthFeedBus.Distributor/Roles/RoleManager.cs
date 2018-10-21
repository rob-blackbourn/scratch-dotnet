using System.Collections.Generic;
using System.Net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Roles
{
    internal class RoleManager
    {
        private readonly DistributorRole _distributorRole;
        private readonly IPAddress _address;
        private readonly string _user;
        private readonly IDictionary<string, IDictionary<Role, bool>> _feedDecision = new Dictionary<string, IDictionary<Role, bool>>();

        internal RoleManager(DistributorRole distributorPermission, IPAddress address, string user)
        {
            _distributorRole = distributorPermission;
            _address = address;
            _user = user;
        }

        internal bool HasRole(string feed, Role role)
        {
            // Check the cache .
            IDictionary<Role, bool> roleDecision;
            if (!_feedDecision.TryGetValue(feed, out roleDecision))
                _feedDecision.Add(feed, roleDecision = new Dictionary<Role, bool>());
            bool decision;
            if (roleDecision.TryGetValue(role, out decision))
                return decision;

            decision = _distributorRole.HasRole(_address, _user, feed, role);

            // Cache the decision;
            roleDecision.Add(role, decision);

            return decision;
        }
    }
}
