using System.Configuration;

namespace JetBlack.MessageBus.FeedBus.Adapters.Configuration
{
    public class FeedBusSection : ConfigurationSection
    {
        #region Connections

        private const string ConnectionsPropertyName = "connections";

        private static readonly ConfigurationProperty ConnectionsProperty = new ConfigurationProperty(ConnectionsPropertyName, typeof(ConnectionElementCollection), null, ConfigurationPropertyOptions.None);

        [ConfigurationProperty(ConnectionsPropertyName, IsRequired = false)]
        public ConnectionElementCollection Connections => (ConnectionElementCollection)this[ConnectionsProperty];

        #endregion

        public static FeedBusSection GetSection()
        {
            return (FeedBusSection)ConfigurationManager.GetSection("feedBus");
        }
    }
}
