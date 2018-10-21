using System;
using System.Net;
using JetBlack.MessageBus.AuthFeedBus.Distributor.Roles;
using JetBlack.MessageBus.AuthFeedBus.Messages;

namespace JetBlack.MessageBus.AuthFeedBus.Distributor.Interactors
{
    public interface  IInteractor : IEquatable<IInteractor>, IComparable<IInteractor>, IDisposable
    {
        Guid Id { get; }
        IPAddress Address { get; }
        string User { get; }
        void SendMessage(Message message);
        Message ReceiveMessage();
        void Start();
        bool HasRole(string feed, Role role);
    }
}
