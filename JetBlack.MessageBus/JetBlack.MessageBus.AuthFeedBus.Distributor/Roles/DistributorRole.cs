using System.Collections.Generic;
using System.Net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Roles
{
    public class DistributorRole
    {
        public DistributorRole(Role allow, Role deny, IReadOnlyDictionary<string, FeedRole> feedRoles)
        {
            Allow = allow;
            Deny = deny;
            FeedRoles = feedRoles ?? new Dictionary<string, FeedRole>();
        }

        public Role Allow { get; }
        public Role Deny { get; }
        public IReadOnlyDictionary<string, FeedRole> FeedRoles { get; }

        public bool HasRole(IPAddress address, string user, string feed, Role role)
        {
            var decision = Allow.HasFlag(role);

            if (Deny.HasFlag(role))
                decision = false;

            FeedRole feedPermission;
            if (FeedRoles.TryGetValue(feed, out feedPermission))
                decision = feedPermission.HasRole(address, user, feed, role, decision);

            return decision;
        }

        public override string ToString()
        {
            return $"Allow={Allow}, Deny={Deny}, FeedRoles=[{string.Join(", ", FeedRoles.Values)}]";
        }
    }
}
