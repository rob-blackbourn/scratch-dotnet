// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;

// namespace JetBlack.MessageBus.Common.IO
// {
//     public static class StreamWriteExtensions
//     {
//         public static void Write(this Stream stream, Int64 value)
//         {
//             var buffer = value.GetBytes();
//             stream.Write(buffer, 0, buffer.Length);
//         }

//         public static async Task WriteAsync(this Stream stream, Int64 value, CancellationToken token)
//         {
//             var buffer = value.GetBytes();
//             await stream.WriteAsync(buffer, 0, buffer.Length, token);
//         }


//         public static void Write(this Stream stream, Int32 value)
//         {
//             var buffer = value.GetBytes();
//             stream.Write(buffer, 0, buffer.Length);
//         }

//         public static async Task WriteAsync(this Stream stream, Int32 value, CancellationToken token)
//         {
//             var buffer = value.GetBytes();
//             await stream.WriteAsync(buffer, 0, buffer.Length, token);
//         }

//         public static void Write(this Stream stream, byte[] value)
//         {
//             if (value == null)
//                 Write(stream, 0);
//             else
//             {
//                 Write(stream, value.Length);
//                 stream.Write(value, 0, value.Length);
//             }
//         }

//         public static async Task WriteAsync(this Stream stream, byte[] value, CancellationToken token)
//         {
//             if (value == null)
//                 await stream.WriteAsync(0, token);
//             else
//             {
//                 await stream.WriteAsync(value.Length, token);
//                 await stream.WriteAsync(value, 0, value.Length, token);
//             }
//         }

//         public static void Write(this Stream stream, IList<byte[]> value)
//         {
//             if (value == null)
//                 Write(stream, 0);
//             else
//             {
//                 Write(stream, value.Count);
//                 foreach (var t in value)
//                     Write(stream, t);
//             }
//         }

//         public static async Task WriteAsync(this Stream stream, IList<byte[]> value, CancellationToken token)
//         {
//             if (value == null)
//                 await stream.WriteAsync(0, token);
//             else
//             {
//                 await stream.WriteAsync(value.Count, token);
//                 foreach (var t in value)
//                     await stream.WriteAsync(t, token);
//             }
//         }

//         public static async Task WriteAsync(this Stream stream, IList<string> values, CancellationToken token)
//         {
//             if (values == null)
//                 await stream.WriteAsync(0, token);
//             else
//             {
//                 await stream.WriteAsync(values.Count, token);
//                 foreach (var value in values)
//                     await stream.WriteAsync(value, token);
//             }
//         }

//         public static void Write(this Stream stream, string value)
//         {
//             stream.Write(value, Encoding.UTF8);
//         }

//         public static void Write(this Stream stream, string value, Encoding encoding)
//         {
//             var bytes = encoding.GetBytes(value);
//             Write(stream, bytes.Length);
//             stream.Write(bytes, 0, bytes.Length);
//         }

//         public static async Task WriteAsync(this Stream stream, string value, CancellationToken token)
//         {
//             await stream.WriteAsync(value, Encoding.UTF8, token);
//         }

//         public static async Task WriteAsync(this Stream stream, string value, Encoding encoding, CancellationToken token)
//         {
//             var bytes = encoding.GetBytes(value);
//             await stream.WriteAsync(bytes.Length, token);
//             await stream.WriteAsync(bytes, 0, bytes.Length, token);
//         }
//     }
// }
