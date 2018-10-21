using System.ComponentModel;
using System.Configuration;
using System.Net;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Configuration
{
    public class InteractorRoleConfig
    {
        public IPAddress Address {get;set;}

        public string User {get;set;}
        public Role Allow {get;set;}
        public Role Deny {get;set;}

        public InteractorRole ToInteractorRole()
        {
            return new InteractorRole(Address, User, Allow, Deny);
        }
    }
}
