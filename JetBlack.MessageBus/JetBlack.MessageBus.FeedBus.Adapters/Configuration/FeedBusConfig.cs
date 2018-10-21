using System.Collections.Generic;
using System.Configuration;

namespace JetBlack.MessageBus.FeedBus.Adapters.Configuration
{
    public class FeedBusConfig
    {
        public Dictionary<string, ConnectionConfig> Connections {get;set;}
    }
}
