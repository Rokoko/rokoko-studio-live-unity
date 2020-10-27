using System;
using System.Runtime.InteropServices;

namespace Rokoko.Core
{
    public class LZ4Wrapper
    {
        public static class LZ4_API
        {
#if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
        private const string LUADLL = "__Internal";
#else
            private const string LUADLL = "lz4";
#endif

            [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Unity_LZ4_compress(IntPtr src, int srcSize, IntPtr dst, int dstCapacity, int compressionLevel);

            [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Unity_LZ4_compressSize(int srcSize, int compressionLevel);

            [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Unity_LZ4_uncompressSize(IntPtr srcBuffer, int srcSize);

            [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Unity_LZ4_decompress(IntPtr src, int srcSize, IntPtr dst, int dstCapacity);
        }

        public static byte[] Compress(byte[] input, int compressionLevel = 3)
        {
            byte[] result = null;

            if (input != null && input.Length > 0)
            {
                int maxSize = LZ4_API.Unity_LZ4_compressSize(input.Length, compressionLevel);
                if (maxSize > 0)
                {
                    var buffer = new byte[maxSize];
                    var srcHandle = GCHandle.Alloc(input, GCHandleType.Pinned);
                    var dstHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    var actualSize = LZ4_API.Unity_LZ4_compress(srcHandle.AddrOfPinnedObject(), input.Length, dstHandle.AddrOfPinnedObject(), maxSize, compressionLevel);

                    if (actualSize > 0)
                    {
                        result = new byte[actualSize];
                        Array.Copy(buffer, result, actualSize);
                    }

                    srcHandle.Free();
                    dstHandle.Free();
                }
            }

            return result;
        }

        public static byte[] Decompress(byte[] input)
        {
            byte[] result = null;

            if (input != null && input.Length > 0)
            {
                var srcHandle = GCHandle.Alloc(input, GCHandleType.Pinned);
                var uncompressSize = LZ4_API.Unity_LZ4_uncompressSize(srcHandle.AddrOfPinnedObject(), input.Length);
                result = new byte[uncompressSize];
                var dstHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
                if (LZ4_API.Unity_LZ4_decompress(srcHandle.AddrOfPinnedObject(), input.Length, dstHandle.AddrOfPinnedObject(), result.Length) != 0)
                {
                    result = null;
                }

                srcHandle.Free();
                dstHandle.Free();
            }

            return result;
        }
    }
}