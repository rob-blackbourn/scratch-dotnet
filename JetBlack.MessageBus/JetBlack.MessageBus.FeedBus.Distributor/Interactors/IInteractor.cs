using System;
using System.Net;
using JetBlack.MessageBus.FeedBus.Messages;

namespace JetBlack.MessageBus.FeedBus.Distributor.Interactors
{
    public interface IInteractor : IDisposable, IEquatable<IInteractor>, IComparable<IInteractor>
    {
        Guid Id { get; }
        IPAddress Address { get; }
        void SendMessage(Message message);
        Message ReceiveMessage();
        void Start();
    }
}
