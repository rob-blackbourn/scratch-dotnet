using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;

namespace JetBlack.MessageBus.FeedBus.Distributor.Configuration
{
    public class DistributorSection
    {
        public IPAddress Address {get;set;}
        public int Port {get;set;}
        public TimeSpan HeartbeaInterval {get;set;}
    }
}
