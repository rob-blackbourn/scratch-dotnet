using System;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    /// <summary>
    /// A data table converter
    /// </summary>
    public class DataTableConverter : JsonConverter
    {
        private readonly StyledDataTableConverter _converter;

        public DataTableConverter(DataTableConversionStyle style = DataTableConversionStyle.Compact)
        {
            switch (style)
            {
                case DataTableConversionStyle.Compact:
                    _converter = new CompactDataTableConverter();
                    break;

                case DataTableConversionStyle.Verbose:
                    _converter = new VerboseDataTableConverter();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            _converter.WriteJson(writer, value, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return _converter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return _converter.CanConvert(objectType);
        }

        public DataTable ToDataTable(JToken token)
        {
            return _converter.ToDataTable(token);
        }
    }
}
