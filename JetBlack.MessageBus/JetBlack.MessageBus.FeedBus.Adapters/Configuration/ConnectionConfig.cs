using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.FeedBus.Adapters.Configuration
{
    public class ConnectionConfig
    {
        public string Name {get;set;}
        public IPAddress Address {get;set;}
        public int Port {get;set;}
        public Type ByteEncoderType {get;set;}

        public override string ToString()
        {
            return $"Name=\"{Name}\", Address={Address}, Port={Port}, ByteEncoderType={ByteEncoderType}";
        }
    }
}
