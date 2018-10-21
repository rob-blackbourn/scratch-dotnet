using System.Collections.Generic;
using System.Configuration;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters.Configuration
{
    public class AuthFeedBusConfig
    {
        public Dictionary<string, ConnectionConfig> Connections {get;set;}
    }
}
