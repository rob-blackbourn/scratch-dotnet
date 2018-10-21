using System.IO;
using System.Net;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public class InteractorAdvertisement : Message
    {
        public InteractorAdvertisement(string user, IPAddress address, bool isJoining)
            : base(MessageType.InteractorAdvertisement)
        {
            User = user;
            Address = address;
            IsJoining = isJoining;
        }

        public string User { get; }
        public IPAddress Address { get; }
        public bool IsJoining { get; }

        public static InteractorAdvertisement ReadBody(Stream stream)
        {
            var user = stream.ReadString();
            var address = stream.ReadIPAddress();
            var isJoining = stream.ReadBoolean();
            return new InteractorAdvertisement(user, address, isJoining);
        }

        public override Stream Write(Stream stream)
        {
            base.Write(stream);
            stream.Write(User);
            stream.Write(Address);
            stream.Write(IsJoining);
            return stream;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, User={User}, Address={Address}, IsJoining={IsJoining}";
        }
    }
}
