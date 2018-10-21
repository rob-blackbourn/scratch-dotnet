// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;

// namespace JetBlack.MessageBus.Common.IO
// {
//     public static class StreamReadExtensions
//     {
//         public static Int32 ReadInt32(this Stream stream)
//         {
//             return stream.ReadFully(new byte[4]).ToInt32();
//         }

//         public static async Task<Int32> ReadInt32Async(this Stream stream, CancellationToken token)
//         {
//             return (await stream.ReadFullyAsync(new byte[4], token)).ToInt32();
//         }

//         public static Int64 ReadInt64(this Stream stream)
//         {
//             return stream.ReadFully(new byte[8]).ToInt64();
//         }

//         public static async Task<Int64> ReadInt64Async(this Stream stream, CancellationToken token)
//         {
//             return (await stream.ReadFullyAsync(new byte[8], token)).ToInt64();
//         }

//         public static byte[] ReadByteArray(this Stream stream)
//         {
//             var nbytes = stream.ReadInt32();
//             var data = new byte[nbytes];
//             var offset = 0;
//             while (nbytes > 0)
//             {
//                 var bytesRead = stream.Read(data, offset, nbytes);
//                 if (bytesRead == 0)
//                     throw new EndOfStreamException();
//                 nbytes -= bytesRead;
//                 offset += bytesRead;
//             }
//             return data;
//         }

//         public static async Task<byte[]> ReadByteArrayAsync(this Stream stream, CancellationToken token)
//         {
//             var nbytes = await stream.ReadInt32Async(token);
//             var data = new byte[nbytes];
//             var offset = 0;
//             while (nbytes > 0)
//             {
//                 var bytesRead = await stream.ReadAsync(data, offset, nbytes, token);
//                 if (bytesRead == 0)
//                     throw new EndOfStreamException();
//                 nbytes -= bytesRead;
//                 offset += bytesRead;
//             }
//             return data;
//         }

//         public static IList<byte[]> ReadArrayOfByteArrays(this Stream stream)
//         {
//             var count = stream.ReadInt32();
//             if (count == 0)
//                 return null;
//             var array = new byte[count][];
//             for (var i = 0; i < count; ++i)
//                 array[i] = stream.ReadByteArray();
//             return array;
//         }

//         public static async Task<IList<byte[]>> ReadArrayOfByteArraysAsync(this Stream stream, CancellationToken token)
//         {
//             var count = await stream.ReadInt32Async(token);
//             if (count == 0)
//                 return null;
//             var array = new byte[count][];
//             for (var i = 0; i < count; ++i)
//                 array[i] = await stream.ReadByteArrayAsync(token);
//             return array;
//         }

//         public static async Task<IList<string>> ReadArrayOfStringsAsync(this Stream stream, CancellationToken token)
//         {
//             var count = await stream.ReadInt32Async(token);
//             if (count == 0)
//                 return null;
//             var array = new string[count];
//             for (var i = 0; i < count; ++i)
//                 array[i] = await stream.ReadStringAsync(token);
//             return array;
//         }

//         public static string ReadString(this Stream stream)
//         {
//             return ReadString(stream, Encoding.UTF8);
//         }

//         public static string ReadString(this Stream stream, Encoding encoding)
//         {
//             var len = stream.ReadInt32();
//             return encoding.GetString(stream.ReadFully(new byte[len]));
//         }

//         public static async Task<string> ReadStringAsync(this Stream stream, CancellationToken token)
//         {
//             return await ReadStringAsync(stream, Encoding.UTF8, token);
//         }

//         public static async Task<string> ReadStringAsync(this Stream stream, Encoding encoding, CancellationToken token)
//         {
//             var len = await stream.ReadInt32Async(token);
//             return encoding.GetString(await stream.ReadFullyAsync(new byte[len], token));
//         }

//         public static byte[] ReadFully(this Stream stream, byte[] buf)
//         {
//             return stream.ReadFully(buf, 0, buf.Length);
//         }

//         public static byte[] ReadFully(this Stream stream, byte[] buf, int off, int len)
//         {
//             if (len < 0)
//                 throw new IndexOutOfRangeException();

//             var n = 0;
//             while (n < len)
//             {
//                 var count = stream.Read(buf, off + n, len - n);
//                 if (count < 0)
//                     throw new EndOfStreamException();
//                 n += count;
//             }

//             return buf;
//         }

//         public static async Task<byte[]> ReadFullyAsync(this Stream stream, byte[] buf, CancellationToken token)
//         {
//             return await stream.ReadFullyAsync(buf, 0, buf.Length, token);
//         }

//         public static async Task<byte[]> ReadFullyAsync(this Stream stream, byte[] buf, int off, int len, CancellationToken token)
//         {
//             if (len < 0)
//                 throw new IndexOutOfRangeException();

//             var n = 0;
//             while (n < len)
//             {
//                 var count = await stream.ReadAsync(buf, off + n, len - n, token);
//                 if (count < 0)
//                     throw new EndOfStreamException();
//                 n += count;
//             }

//             return buf;
//         }
//     }
// }
