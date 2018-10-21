using System.Configuration;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters.Configuration
{
    public class AuthFeedBusSection : ConfigurationSection
    {
        #region Connections

        private const string ConnectionsPropertyName = "connections";

        private static readonly ConfigurationProperty ConnectionsProperty = new ConfigurationProperty(ConnectionsPropertyName, typeof(ConnectionElementCollection), null, ConfigurationPropertyOptions.None);

        [ConfigurationProperty(ConnectionsPropertyName, IsRequired = false)]
        public ConnectionElementCollection Connections => (ConnectionElementCollection)this[ConnectionsProperty];

        #endregion

        public static AuthFeedBusSection GetSection()
        {
            return (AuthFeedBusSection)ConfigurationManager.GetSection("authFeedBus");
        }
    }
}
