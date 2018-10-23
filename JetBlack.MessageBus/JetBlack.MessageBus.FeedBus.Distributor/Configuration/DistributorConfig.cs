using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;

namespace JetBlack.MessageBus.FeedBus.Distributor.Configuration
{
    public class DistributorConfig
    {
        public string Address {get;set;}
        public int Port {get;set;}
        public TimeSpan HeartbeatInterval {get;set;}
    }
}
