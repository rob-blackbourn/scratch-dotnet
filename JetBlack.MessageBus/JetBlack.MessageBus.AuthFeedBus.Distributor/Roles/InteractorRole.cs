using System;
using System.Net;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Roles
{
    public class InteractorRole
    {
        public InteractorRole(IPAddress address, string user, Role allow, Role deny)
        {
            Address = address;
            User = user;
            Allow = allow;
            Deny = deny;
        }

        public IPAddress Address { get; }
        public string User { get; }
        public Role Allow { get; }
        public Role Deny { get; }

        public bool HasRole(Role role, bool decision)
        {
            if (Allow.HasFlag(role))
                decision = true;

            if (Deny.HasFlag(role))
                decision = false;

            return decision;
        }

        public override string ToString()
        {
            return $"Address={Address}, User=\"{User}\", Allow={Allow}, Deny={Deny}";
        }

        public struct Key : IEquatable<Key>, IComparable<Key>
        {
            public Key(IPAddress address, string user)
            {
                Address = address;
                User = user;
            }

            public IPAddress Address { get; }
            public string User { get; }

            public int CompareTo(Key other)
            {
                var diff = string.Compare((Address ?? IPAddress.Any).ToString(), (other.Address ?? IPAddress.Any).ToString(), StringComparison.Ordinal);
                if (diff == 0)
                    diff = string.Compare((User ?? string.Empty), other.User ?? string.Empty, StringComparison.Ordinal);
                return diff;
            }

            public bool Equals(Key other)
            {
                return Equals(Address, other.Address) && User == other.User;
            }

            public override bool Equals(object obj)
            {
                return obj is Key && Equals((Key)obj);
            }

            public override int GetHashCode()
            {
                return (Address == null ? 0 : Address.GetHashCode()) ^ (User == null ? 0 : User.GetHashCode());
            }

            public override string ToString()
            {
                return $"Address={Address}, User={User}";
            }
        }
    }
}
