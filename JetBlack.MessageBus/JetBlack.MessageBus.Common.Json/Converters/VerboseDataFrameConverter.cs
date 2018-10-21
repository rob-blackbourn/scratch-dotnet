using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Common.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    public class VerboseDataFrameConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataFrame = (DataFrame)value;

            // [
            writer.WriteStartArray();

            foreach (var row in dataFrame.Rows)
            {
                // {
                writer.WriteStartObject();

                for (var i = 0; i < row.Value.Length; ++i)
                {
                    writer.WritePropertyName(dataFrame.Columns[i].Name);
                    serializer.Serialize(writer, row.Value[i]);
                }

                // }
                writer.WriteEndObject();
            }

            // ]
            writer.WriteEndArray();
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ToDataFrame(JArray.Load(reader));
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataFrame).IsAssignableFrom(objectType);
        }

        private static DataFrame ToDataFrame(JArray obj)
        {
            var dataFrame = new DataFrame();

            var data = new Dictionary<string,List<object>>();
            foreach (var row in obj.Cast<JObject>())
            {
                foreach (var property in row)
                {
                    if (!data.TryGetValue(property.Key, out var list))
                        list = new List<object>();
                    list.Add(((JValue)property.Value).Value);
                }
            }

            foreach (var item in data)
                dataFrame.Columns.Add(new Vector<object>(item.Value, item.Key));

            return dataFrame;
        }
    }
}
