using System;
using System.Text;
using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Common.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JetBlack.MessageBus.Common.Json
{
    public class JsonByteEncoder : IByteEncoder
    {
        private readonly JsonSerializer _serializer;

        public JsonByteEncoder()
        {
            _serializer = JsonSerializer.Create();
            _serializer.Converters.Add(new VectorConverter());
            _serializer.Converters.Add(new DataFrameConverter());
            _serializer.Converters.Add(new DataTableConverter());
            _serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public object Decode(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var json = Encoding.UTF8.GetString(bytes);

            var frame = _serializer.Deserialize<JsonFrame>(json);
            var type = Type.GetType(frame.MetaData);
            if (type == null)
                throw new ApplicationException($"Unable to deserialize unknown type \"{frame.MetaData}\"");

            var obj = _serializer.Deserialize(frame.Data, type);

            return obj;
        }

        public byte[] Encode(object data)
        {
            if (data == null)
                return null;

            var frame = new JsonFrame
            {
                MetaData = data.GetType().AssemblyQualifiedName ?? "null",
                Data = _serializer.Serialize(data)
            };

            var json = _serializer.Serialize(frame);

            return Encoding.UTF8.GetBytes(json);
        }
    }

    public class JsonFrame
    {
        public string MetaData { get; set; }
        public string Data { get; set; }
    }
}
