using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Configuration
{
    public class FeedRoleConfig
    {
        public string Feed {get;set;}
        public Role Allow {get;set;}
        public Role Deny {get;set;}
        public bool RequiresEntitlement {get;set;}
        public List<InteractorRoleConfig> InteractorRoles {get;set;}

        public FeedRole ToFeedRole()
        {
            return new FeedRole(
                Feed,
                Allow,
                Deny,
                RequiresEntitlement,
                InteractorRoles.Select(x => x.ToInteractorRole()).ToList());
        }
    }
}
