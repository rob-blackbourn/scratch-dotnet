using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Roles
{
    public class FeedRole
    {
        public FeedRole(string feed, Role allow, Role deny, bool requiresEntitlement, IEnumerable<InteractorRole> interactorRoles)
        {
            Feed = feed;
            Allow = allow;
            Deny = deny;
            RequiresEntitlement = requiresEntitlement;
            InteractorRoles = (interactorRoles ?? new InteractorRole[0]).ToDictionary(x => new InteractorRole.Key(x.Address, x.User));
        }

        public string Feed { get; }
        public Role Allow { get; }
        public Role Deny { get; }
        public bool RequiresEntitlement { get; }
        public IReadOnlyDictionary<InteractorRole.Key, InteractorRole> InteractorRoles { get; }

        public bool HasRole(IPAddress address, string user, string feed, Role role, bool decision)
        {
            if (Allow.HasFlag(role))
                decision = true;

            if (Deny.HasFlag(role))
                decision = false;

            InteractorRole interactorRole;
            if (InteractorRoles.TryGetValue(new InteractorRole.Key(address, user), out interactorRole))
                decision = interactorRole.HasRole(role, decision);

            return decision;
        }

        public override string ToString()
        {
            return $"Feed={Feed}, Allow={Allow}, Deny={Deny}, RequiresEntitlement={RequiresEntitlement}, InteractorRoles=[{string.Join(", ", InteractorRoles.Values)}]";
        }
    }
}
