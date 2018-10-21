using System;
using System.IO;
using System.Net;
using System.Text;

namespace JetBlack.MessageBus.Common.IO
{
    /// <summary>
    /// Extension methods for streaming data.
    /// </summary>
    public static class StreamExtensions
    {
        #region Reading

        /// <summary>
        /// Read an array of bytes from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <param name="value">The buffer to hold the data</param>
        /// <returns>The count of bytes read</returns>
        public static int Read(this Stream stream, byte[] value)
        {
            return stream.Read(value, 0, value.Length);
        }

        /// <summary>
        /// Read an array of bytes from a stream into an array at a given offset.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <param name="value">The buffer to hold the data</param>
        /// <param name="off">The offset into the array where the data will be written</param>
        /// <param name="len">The number of bytes to read</param>
        /// <returns>The count of bytes read</returns>
        public static int Read(this Stream stream, byte[] value, int off, int len)
        {
            return stream.Read(value, off, len);
        }

        /// <summary>
        /// Read a boolean from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The boolean read.</returns>
        public static bool ReadBoolean(this Stream stream)
        {
            return (stream.ReadByte() != 0);
        }

        /// <summary>
        /// Read a byte from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The byte read</returns>
        public static byte ReadByte(this Stream stream)
        {
            var ch = stream.ReadByte();
            if (ch < 0)
                throw new EndOfStreamException();
            return (byte)ch;
        }

        /// <summary>
        /// Read a character from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The character read</returns>
        public static char ReadChar(this Stream stream)
        {
            return stream.ReadFully(new byte[2]).ToChar();
        }

        /// <summary>
        /// Read a double from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The double read</returns>
        public static double ReadDouble(this Stream stream)
        {
            return BitConverter.Int64BitsToDouble(stream.ReadInt64());
        }

        /// <summary>
        /// Read a float from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The float read</returns>
        public static float ReadFloat(this Stream stream)
        {
            return stream.ReadFully(new byte[4]).ToFloat();
        }

        /// <summary>
        /// Read a short from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The short read</returns>
        public static short ReadInt16(this Stream stream)
        {
            return stream.ReadFully(new byte[2]).ToInt16();
        }

        /// <summary>
        /// Read an unsigned short from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The unsigned short read</returns>
        public static ushort ReadUInt16(this Stream stream)
        {
            return (ushort)stream.ReadFully(new byte[2]).ToInt16();
        }

        /// <summary>
        /// Read an int from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The int read</returns>
        public static int ReadInt32(this Stream stream)
        {
            return stream.ReadFully(new byte[4]).ToInt32();
        }

        /// <summary>
        /// Read a long from a stream
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The long read</returns>
        public static long ReadInt64(this Stream stream)
        {
            return stream.ReadFully(new byte[8]).ToInt64();
        }

        /// <summary>
        /// Read an ip address from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The ip address read</returns>
        public static IPAddress ReadIPAddress(this Stream stream)
        {
            var len = stream.ReadInt32();
            var address = new byte[len];
            stream.ReadFully(address);
            return new IPAddress(address);
        }

        /// <summary>
        /// Read a string from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The string read</returns>
        public static string ReadString(this Stream stream)
        {
            var len = stream.ReadInt32();
            return Encoding.UTF8.GetString(stream.ReadFully(new byte[len]));
        }

        /// <summary>
        /// Read an array of strings from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The array of strings read</returns>
        public static string[] ReadArrayOfStrings(this Stream stream)
        {
            var count = stream.ReadInt32();
            if (count == 0)
                return null;
            var array = new string[count];
            for (var i = 0; i < count; ++i)
                array[i] = stream.ReadString();
            return array;
        }

        /// <summary>
        /// Read an array of bytes from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The array of bytes read</returns>
        public static byte[] ReadByteArray(this Stream stream)
        {
            var nbytes = stream.ReadInt32();
            var data = new byte[nbytes];
            var offset = 0;
            while (nbytes > 0)
            {
                var bytesRead = stream.Read(data, offset, nbytes);
                if (bytesRead == 0)
                    throw new EndOfStreamException();
                nbytes -= bytesRead;
                offset += bytesRead;
            }
            return data;
        }

        /// <summary>
        /// Read an array of byte arrays from a stream.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The array of byte arrays read</returns>
        public static byte[][] ReadArrayOfByteArrays(this Stream stream)
        {
            var count = stream.ReadInt32();
            if (count == 0)
                return null;
            var array = new byte[count][];
            for (var i = 0; i < count; ++i)
                array[i] = stream.ReadByteArray();
            return array;
        }

        /// <summary>
        /// Read a guid from a stream
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The guid read</returns>
        public static Guid ReadGuid(this Stream stream)
        {
            return new Guid(stream.ReadByteArray());
        }

        /// <summary>
        /// Read an array of guids from a stream
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The array of guids read</returns>
        public static Guid[] ReadGuidArray(this Stream stream)
        {
            var count = stream.ReadInt32();
            if (count == 0)
                return null;

            var array = new Guid[count];
            for (var i = 0; i < count; ++i)
                array[i] = stream.ReadGuid();

            return array;
        }

        private static byte[] ReadFully(this Stream stream, byte[] buf)
        {
            return stream.ReadFully(buf, 0, buf.Length);
        }

        private static byte[] ReadFully(this Stream stream, byte[] buf, int off, int len)
        {
            if (len < 0)
                throw new IndexOutOfRangeException();

            var n = 0;
            while (n < len)
            {
                var count = stream.Read(buf, off + n, len - n);
                if (count < 0)
                    throw new EndOfStreamException();
                n += count;
            }

            return buf;
        }

        /// <summary>
        /// Read a date from a stream
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The date read</returns>
        public static DateTime ReadDate(this Stream stream)
        {
            return stream.ReadInt64().Int64ToDate();
        }

        #endregion

        #region Writing

        /// <summary>
        /// Write a boolean to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, bool value)
        {
            stream.WriteByte((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// Write a byte to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, byte value)
        {
            stream.WriteByte(value);
        }

        /// <summary>
        /// Write a character to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, char value)
        {
            stream.Write(value.GetBytes(), 0, 2);
        }

        /// <summary>
        /// Write an int to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, int value)
        {
            stream.Write(value.GetBytes(), 0, 4);
        }

        /// <summary>
        /// Write a long to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, long value)
        {
            stream.Write(value.GetBytes(), 0, 8);
        }

        /// <summary>
        /// Write a short to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, short value)
        {
            stream.Write(value.GetBytes(), 0, 2);
        }

        /// <summary>
        /// Write an unsigned short to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, ushort value)
        {
            stream.Write(PortableBitConverter.GetBytes(value), 0, 2);
        }

        /// <summary>
        /// Write a float to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, float value)
        {
            stream.Write(value.GetBytes(), 0, 4);
        }

        /// <summary>
        /// Write a double to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, double value)
        {
            stream.Write(BitConverter.DoubleToInt64Bits(value));
        }

        /// <summary>
        /// Write a string to a stream.
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            stream.Write(bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Write a date to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, DateTime value)
        {
            stream.Write(value.DateToInt64());
        }

        /// <summary>
        /// Write an ip address to a stream.
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, IPAddress value)
        {
            byte[] address = value.GetAddressBytes();
            stream.Write(address.Length);
            stream.Write(address, 0, address.Length);
        }

        /// <summary>
        /// Write a byte array to a stream.
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, byte[] value)
        {
            if (value == null)
                stream.Write(0);
            else
            {
                stream.Write(value.Length);
                stream.Write(value, 0, value.Length);
            }
        }

        /// <summary>
        /// Write an array of byte arrays to a stream.
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, byte[][] value)
        {
            if (value == null)
                stream.Write(0);
            else
            {
                stream.Write(value.Length);
                foreach (var t in value)
                    stream.Write(t);
            }
        }

        /// <summary>
        /// Write an array of stings to a stream
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, string[] value)
        {
            if (value == null)
                stream.Write(0);
            else
            {
                stream.Write(value.Length);
                foreach (var t in value)
                    stream.Write(t);
            }
        }

        /// <summary>
        /// Write a guid to a stream.
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, Guid value)
        {
            stream.Write(value.ToByteArray());
        }

        /// <summary>
        /// Write an array of guids to a stream.
        /// </summary>
        /// <param name="stream">The sink stream</param>
        /// <param name="value">The value to write</param>
        public static void Write(this Stream stream, Guid[] value)
        {
            if (value == null)
                stream.Write(0);
            else
            {
                stream.Write(value.Length);
                foreach (var guid in value)
                    stream.Write(guid);
            }
        }

        #endregion
    }
}
