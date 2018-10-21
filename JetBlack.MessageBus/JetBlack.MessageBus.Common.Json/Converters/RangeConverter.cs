using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBlack.MessageBus.Common.Collections.Generic;
using Newtonsoft.Json;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    public class RangeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rangeType = value.GetType();
            var start = rangeType.GetProperty("Start").GetValue(value);
            var end = rangeType.GetProperty("End").GetValue(value);
            var isStartIncluded = (bool)rangeType.GetProperty("IsStartIncluded").GetValue(value);
            var isEndIncluded = (bool)rangeType.GetProperty("IsEndIncluded").GetValue(value);

            // {
            writer.WriteStartObject();

            writer.WritePropertyName("start");
            serializer.Serialize(writer, start);

            writer.WritePropertyName("end");
            serializer.Serialize(writer, end);

            if (!isStartIncluded)
            {
                writer.WritePropertyName("isStartIncluded");
                writer.WriteValue(isStartIncluded);
            }

            if (!isEndIncluded)
            {
                writer.WritePropertyName("isEndIncluded");
                writer.WriteValue(isEndIncluded);
            }

            // }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var rangeTypeInfo = objectType.GetTypeInfo();

            if (reader.TokenType == JsonToken.Null)
            {
                if (rangeTypeInfo.IsGenericType && rangeTypeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return null;

                throw new JsonSerializationException("Cannot convert null value to Range<>.");
            }

            if (rangeTypeInfo.IsGenericType && rangeTypeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                rangeTypeInfo = Nullable.GetUnderlyingType(objectType).GetTypeInfo();

            var valueType = rangeTypeInfo.GetGenericArguments().Single();

            object start = null, end = null, isStartIncluded = true, isEndIncluded = true;

            reader.Read();
            while (reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value.ToString();

                if (string.Equals(propertyName, "start", StringComparison.OrdinalIgnoreCase))
                {
                    reader.Read();
                    start = serializer.Deserialize(reader, valueType);
                }
                else if (string.Equals(propertyName, "end", StringComparison.OrdinalIgnoreCase))
                {
                    reader.Read();
                    end = serializer.Deserialize(reader, valueType);
                }
                else if (string.Equals(propertyName, "isStartIncluded", StringComparison.OrdinalIgnoreCase))
                {
                    isStartIncluded = reader.ReadAsBoolean();
                }
                else if (string.Equals(propertyName, "isEndIncluded", StringComparison.OrdinalIgnoreCase))
                {
                    isEndIncluded = reader.ReadAsBoolean();
                }
                else
                    throw new JsonSerializationException($"Unexpected property \"{propertyName}\".");

                reader.Read();
            }

            if (start == null || end == null)
                throw new JsonSerializationException("Invalid range");

            return CreateRange(rangeTypeInfo.AsType(), valueType, start, end, isStartIncluded, isEndIncluded);
        }

        public override bool CanConvert(Type objectType)
        {
            var typeInfo = objectType.GetTypeInfo();

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                typeInfo = Nullable.GetUnderlyingType(objectType).GetTypeInfo();

            if (typeInfo.IsValueType && typeInfo.IsGenericType)
                return typeInfo.GetGenericTypeDefinition() == typeof(Range<>);

            return false;
        }

        public static object CreateRange(Type rangeType, Type valueType, object start, object end, object isStartIncluded, object isEndIncluded)
        {
            var comparerType = typeof(IComparer<>).MakeGenericType(valueType);
            var constructor = rangeType.GetConstructor(new[] { valueType, valueType, comparerType, typeof(bool), typeof(bool) });
            return constructor.Invoke(new[] { start, end, null, isStartIncluded, isEndIncluded });
        }
    }
}
