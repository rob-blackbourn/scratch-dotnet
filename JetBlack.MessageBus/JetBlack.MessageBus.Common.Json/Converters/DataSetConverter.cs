using System;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    /// <summary>
    /// A custom converter for a data set.
    /// </summary>
    public class DataSetConverter : JsonConverter
    {
        private readonly DataTableConverter _converter;

        public DataSetConverter(DataTableConversionStyle style = DataTableConversionStyle.Compact)
        {
            _converter = new DataTableConverter(style);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object dataset, JsonSerializer serializer)
        {
            var dataSet = (DataSet)dataset;

            writer.WriteStartArray();

            foreach (DataTable table in dataSet.Tables)
            {
                _converter.WriteJson(writer, table, serializer);
            }
            writer.WriteEndArray();
        }

        /// <inheritdoc />
        public override bool CanConvert(Type valueType)
        {
            return typeof(DataSet).IsAssignableFrom(valueType);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dataSet = new DataSet();

            var tables = JArray.Load(reader);
            foreach (var table in tables.AsJEnumerable())
                dataSet.Tables.Add(_converter.ToDataTable(table));

            return dataSet;
        }
    }
}