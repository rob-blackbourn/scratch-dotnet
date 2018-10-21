using System;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    public class CompactDataTableConverter : StyledDataTableConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object dataTable, JsonSerializer serializer)
        {
            var table = (DataTable)dataTable;

            // {
            writer.WriteStartObject();

            writer.WritePropertyName("name");
            serializer.Serialize(writer, table.TableName);

            writer.WritePropertyName("columns");

            // [
            writer.WriteStartArray();
            foreach (DataColumn column in table.Columns)
            {
                // {
                writer.WriteStartObject();

                writer.WritePropertyName("name");
                serializer.Serialize(writer, column.ColumnName);

                writer.WritePropertyName("type");
                serializer.Serialize(writer, Type.GetTypeCode(column.DataType).ToString());

                writer.WritePropertyName("nullable");
                serializer.Serialize(writer, column.AllowDBNull);

                writer.WriteEndObject();
                // }
            }
            writer.WriteEndArray();
            // ]

            writer.WritePropertyName("rows");

            // [
            writer.WriteStartArray();

            foreach (DataRow row in table.Rows)
            {
                writer.WriteStartArray();
                foreach (var value in row.ItemArray)
                    serializer.Serialize(writer, value);
                writer.WriteEndArray();
            }

            writer.WriteEndArray();
            // ]

            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override bool CanConvert(Type valueType)
        {
            return typeof(DataTable).IsAssignableFrom(valueType);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ToDataTable(JObject.Load(reader));
        }


        /// <summary>
        /// Convert a json object to a data table
        /// </summary>
        /// <param name="token">The json representation of the data table.</param>
        /// <returns>A data table.</returns>
        public override DataTable ToDataTable(JToken token)
        {
            var table = (JObject) token;

            var tableName = (string)table["name"];
            var columns = (JArray)table["columns"];
            var rows = (JArray)table["rows"];

            var dataTable = new DataTable(tableName);
            foreach (var column in columns.OfType<JObject>())
            {
                var columnType = Type.GetType("System." + (string)column["type"]);
                var dataColumn = new DataColumn((string)column["name"]) { AllowDBNull = (bool)column["nullable"] };
                if (columnType != null)
                    dataColumn.DataType = columnType;
                dataTable.Columns.Add(dataColumn);
            }

            foreach (var row in rows.OfType<JArray>())
            {
                var dataRow = dataTable.NewRow();
                for (var i = 0; i < row.Count; ++i)
                    dataRow[i] = ((JValue)row[i]).Value ?? DBNull.Value;
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
    }
}
