using System;
using JetBlack.MessageBus.Common.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    /// <summary>
    /// A converter for vectors.
    /// </summary>
    public class VectorConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (IVector) value;

            // {
            writer.WriteStartObject();

            writer.WritePropertyName("name");
            serializer.Serialize(writer, vector.Name);

            writer.WritePropertyName("type");
            serializer.Serialize(writer, Type.GetTypeCode(vector.Type).ToString());

            writer.WritePropertyName("nullable");
            serializer.Serialize(writer, vector.IsNullable);

            writer.WritePropertyName("values");
            // [
            writer.WriteStartArray();
            foreach (var item in vector)
                serializer.Serialize(writer, item);
            // ]
            writer.WriteEndArray();

            // }
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ToVector(JObject.Load(reader));
        }

        /// <summary>
        /// Convert a json object to a vector.
        /// </summary>
        /// <param name="obj">The json representation of the vector.</param>
        /// <returns>A verctor</returns>
        public static IVector ToVector(JObject obj)
        {
            var name = (string) obj["name"];
            var type = Type.GetType("System." + (string)obj["type"]);
            var nullable = (bool) obj["nullable"];
            var values = (JArray)obj["values"];
            var vector = values.CreateVector(type, name, value => Convert.ChangeType(((JValue)value).Value, type), nullable);
            return vector;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(IVector).IsAssignableFrom(objectType);
        }
    }
}
