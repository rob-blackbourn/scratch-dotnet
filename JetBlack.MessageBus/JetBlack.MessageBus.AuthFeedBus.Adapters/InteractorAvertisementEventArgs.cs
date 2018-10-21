using System;
using System.Net;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters
{
    public class InteractorAvertisementEventArgs : EventArgs
    {
        public InteractorAvertisementEventArgs(string user, IPAddress address, bool isJoining)
        {
            User = user;
            Address = address;
            IsJoining = isJoining;
        }

        public string User { get; }
        public IPAddress Address { get; }
        public bool IsJoining { get; }
    }
}
