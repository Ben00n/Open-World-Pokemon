#if GRIFFIN
using Lzf;


namespace Pinwheel.Griffin.Compression
{
    public static class GCompressor
    {
        private static byte[] outputBuffer;
        private static byte[] OutputBuffer
        {
            get
            {
                int bufferSizeMB = 250;
                if (outputBuffer == null ||
                    outputBuffer.Length != bufferSizeMB * 1000000)
                {
                    outputBuffer = new byte[bufferSizeMB * 1000000];
                }
                return outputBuffer;
            }
        }

        public static byte[] Compress(byte[] data)
        {
            if (data.Length == 0)
                return data;

            LZF compressor = new LZF();
            int compressedLength = compressor.Compress(data, data.Length, OutputBuffer, OutputBuffer.Length);

            byte[] result = new byte[compressedLength];
            System.Array.Copy(OutputBuffer, result, compressedLength);
            return result;
        }

        public static byte[] Decompress(byte[] data)
        {
            if (data.Length == 0)
                return data;

            LZF decompressor = new LZF();
            int decompressedLength = decompressor.Decompress(data, data.Length, OutputBuffer, OutputBuffer.Length);

            byte[] result = new byte[decompressedLength];
            System.Array.Copy(OutputBuffer, result, decompressedLength);
            return result;
        }

        public static void CleanUp()
        {
            outputBuffer = null;
        }
    }
}
#endif
