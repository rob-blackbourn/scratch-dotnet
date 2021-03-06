﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JetBlack.MessageBus.Common.IO
{
    /// <summary>
    /// An encoder which uses the BinaryFormatter.
    /// </summary>
    public class BinaryEncoder : IByteEncoder
    {
        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

        /// <inheritdoc />
        public object Decode(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
                return null;

            using (var stream = new MemoryStream(bytes))
            {
                return BinaryFormatter.Deserialize(stream);
            }
        }

        /// <inheritdoc />
        public byte[] Encode(object obj)
        {
            using (var stream = new MemoryStream())
            {
                BinaryFormatter.Serialize(stream, obj);
                stream.Flush();
                return stream.GetBuffer();
            }
        }
    }
}
