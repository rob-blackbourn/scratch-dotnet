using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Common.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    public class CompactDataFrameConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataFrame = (DataFrame)value;

            // {
            writer.WriteStartObject();

            writer.WritePropertyName("name");
            serializer.Serialize(writer, dataFrame.Name);

            writer.WritePropertyName("rowHeaders");
            writer.WriteStartArray();
            foreach (var header in dataFrame.RowHeaders)
                serializer.Serialize(writer, header);
            writer.WriteEndAsync();

            writer.WritePropertyName("columns");
            serializer.Serialize(writer, dataFrame.Columns);

            // }
            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ToDataFrame(JObject.Load(reader));
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataFrame).IsAssignableFrom(objectType);
        }

        private static DataFrame ToDataFrame(JObject obj)
        {
            var name = (string)obj["name"];
            var rowHeaders = ((JArray)obj["rowHeaders"]).OfType<JValue>().Select(x => (string)x.Value).ToList();
            var columns = (JArray)obj["columns"];

            var dataFrame = new DataFrame(
                name,
                new List<string>(rowHeaders),
                columns.Select(x => VectorConverter.ToVector((JObject)x)).ToArray());

            return dataFrame;
        }
    }
}
