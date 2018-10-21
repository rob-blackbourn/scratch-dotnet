using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Configuration
{
    public class DistributorConfig
    {
        public IPAddress Address {get;set;}
        public int Port {get;set;}
        public TimeSpan HeartbeatInterval {get;set;}
        public Role Allow {get;set;}
        public Role Deny {get;set;}
        public List<FeedRoleConfig> FeedRoles {get;set;}

        public DistributorRole ToDistributorRole()
        {
            return new DistributorRole(
                Allow,
                Deny,
                FeedRoles.Select(x => x.ToFeedRole()).ToDictionary(x => x.Feed, x => x));
        }
    }
}
