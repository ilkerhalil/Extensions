using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Extensions.StreamExtensions {
    using System;

    public static class StreamExtensions {
        private static byte[] CompressToByteArray(this Stream input) {
            using (var compressStream = new MemoryStream())
            using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress)) {
                input.CopyTo(compressor);
                compressor.Close();
                return compressStream.ToArray();
            }
        }
        public static string Compress(this FileInfo fileToCompress, string path) {
            var newFile = fileToCompress.Name + ".gz";
            var fullPath = Path.Combine(path, newFile);
            using (var originalFileStream = fileToCompress.OpenRead()) {
                if (
                    !((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden &
                      fileToCompress.Extension != ".gz")) return fullPath;
                using (var compressedFileStream = File.Create(fullPath)) {
                    using (var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress)) {
                        originalFileStream.CopyTo(compressionStream);
                    }
                }
            }
            return fullPath;
        }
        public static void Decompress(FileInfo fileToDecompress) {
            using (var originalFileStream = fileToDecompress.OpenRead()) {
                var currentFileName = fileToDecompress.FullName;
                var newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (var decompressedFileStream = File.Create(newFileName)) {
                    using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress)) {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Debug.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }
        }

        public static byte[] ReadToEnd(this Stream stream) {
            if (stream == null) throw new ArgumentNullException("stream");
            long originalPosition = 0;
            if (stream.CanSeek) {
                originalPosition = stream.Position;
                stream.Position = 0;
            }
            try {
                var readBuffer = new byte[4096];
                var totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0) {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead != readBuffer.Length) continue;
                    var nextByte = stream.ReadByte();
                    if (nextByte == -1) continue;
                    var temp = new byte[readBuffer.Length * 2];
                    Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                    Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                    readBuffer = temp;
                    totalBytesRead++;
                }

                var buffer = readBuffer;
                if (readBuffer.Length == totalBytesRead) return buffer;
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                return buffer;
            }
            finally {
                if (stream.CanSeek) {
                    stream.Position = originalPosition;
                }
            }
        }


    }
}
