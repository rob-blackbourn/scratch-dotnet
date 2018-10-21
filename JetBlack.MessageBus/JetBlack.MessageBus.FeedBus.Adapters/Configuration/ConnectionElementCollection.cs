using System.Configuration;

namespace JetBlack.MessageBus.FeedBus.Adapters.Configuration
{
    [ConfigurationCollection(typeof(ConnectionElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ConnectionElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConnectionElement)element).Name;
        }

        public new ConnectionElement this[string name] => (ConnectionElement)BaseGet(name);
    }
}
