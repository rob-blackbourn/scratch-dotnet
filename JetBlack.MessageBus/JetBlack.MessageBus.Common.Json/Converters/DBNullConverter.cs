using System;
using Newtonsoft.Json;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    // ReSharper disable once InconsistentNaming
    public class DBNullConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteNull();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DBNull);
        }
    }
}
