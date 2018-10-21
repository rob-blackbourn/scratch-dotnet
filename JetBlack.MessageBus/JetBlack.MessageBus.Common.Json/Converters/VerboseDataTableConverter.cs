using System;
using System.Data;
using System.Linq;
using JetBlack.MessageBus.Common.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JetBlack.MessageBus.Common.Json.Converters
{
    public class VerboseDataTableConverter : StyledDataTableConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataTable = (DataTable)value;

            // [
            writer.WriteStartArray();

            foreach (DataRow row in dataTable.Rows)
            {
                // {
                writer.WriteStartObject();

                foreach (DataColumn column in dataTable.Columns)
                {
                    writer.WritePropertyName(column.ColumnName);
                    serializer.Serialize(writer, row[column]);
                }

                // }
                writer.WriteEndObject();
            }

            // ]
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ToDataTable(JArray.Load(reader));
        }

        private static readonly Type[] NumberTypes = { typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(decimal), typeof(float), typeof(double) };

        private static bool IsPreferredType(Type currentType, Type candidateType)
        {
            var currentTypeIndex = NumberTypes.IndexOf(x => x == currentType);
            var candidateTypeIndex = NumberTypes.IndexOf(x => x == candidateType);
            return currentTypeIndex < candidateTypeIndex ||
                currentType == typeof(object) && candidateType != typeof(object);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DataTable).IsAssignableFrom(objectType);
        }

        public override DataTable ToDataTable(JToken token)
        {
            var array = (JArray) token;

            var dictionaries = array
                .OfType<JObject>()
                .Select(row => row.OfType<JProperty>().ToDictionary(x => x.Name, x => x.Value.ToObject<object>()))
                .ToList();

            var dataTable = new DataTable();
            foreach (var dictionary in dictionaries)
            {
                foreach (var item in dictionary)
                {
                    if (!dataTable.Columns.Contains(item.Key))
                        dataTable.Columns.Add(new DataColumn(item.Key, item.Value?.GetType() ?? typeof(object)) { AllowDBNull = false });
                    else
                    {
                        var dataColumn = dataTable.Columns[item.Key];
                        if (item.Value == null)
                            dataColumn.AllowDBNull = true;
                        else if (IsPreferredType(dataColumn.DataType, item.Value.GetType()))
                            dataColumn.DataType = item.Value.GetType();
                    }
                }
            }

            foreach (var dictionary in dictionaries)
            {
                var row = dataTable.NewRow();
                foreach (var dataColumn in dataTable.Columns.OfType<DataColumn>())
                {
                    if (dictionary.TryGetValue(dataColumn.ColumnName, out var value))
                        row[dataColumn] = value ?? DBNull.Value;
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
