using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using JetBlack.MessageBus.Common.Configuration.Converters;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.AuthFeedBus.Adapters.Configuration
{
    public class ConnectionElement : ConfigurationElement
    {
        #region Name

        private const string NamePropertyName = "name";

        private static readonly ConfigurationProperty NameProperty = new ConfigurationProperty(NamePropertyName, typeof(string), null, ConfigurationPropertyOptions.IsRequired);

        [ConfigurationProperty(NamePropertyName, IsRequired = true)]
        public string Name => (string)this[NameProperty];

        #endregion

        #region Address

        private const string AddressPropertyName = "address";

        private static readonly ConfigurationProperty AddressProperty = new ConfigurationProperty(AddressPropertyName, typeof(IPAddress), IPAddress.None, ConfigurationPropertyOptions.IsRequired);

        [ConfigurationProperty(AddressPropertyName, IsRequired = true), TypeConverter(typeof(IPAddressConverter))]
        public IPAddress Address => (IPAddress)this[AddressProperty];

        #endregion

        #region Port

        private const string PortPropertyName = "port";

        private static readonly ConfigurationProperty PortProperty = new ConfigurationProperty(PortPropertyName, typeof(int), -1, ConfigurationPropertyOptions.IsRequired);

        [ConfigurationProperty(PortPropertyName, IsRequired = true)]
        public int Port => (int)this[PortProperty];

        #endregion

        #region ByteEncoderType

        private const string ByteEncoderTypePropertyName = "byteEncoderType";

        private static readonly ConfigurationProperty ByteEncoderTypeProperty = new ConfigurationProperty(ByteEncoderTypePropertyName, typeof(Type), typeof(BinaryEncoder), ConfigurationPropertyOptions.None);

        [ConfigurationProperty(ByteEncoderTypePropertyName, IsRequired = false), TypeConverter(typeof(TypeNameConverter))]
        public Type ByteEncoderType => (Type)this[ByteEncoderTypeProperty];

        #endregion

        public override string ToString()
        {
            return $"Name=\"{Name}\", Address={Address}, Port={Port}, ByteEncoderType={ByteEncoderType}";
        }
    }
}
