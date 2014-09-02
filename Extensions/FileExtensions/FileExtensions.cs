namespace Extensions.FileExtensions
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public static class FileExtensions
    {
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static UInt32 FindMimeFromData(
            UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed,
            UInt32 dwMimeFlags,
            out System.UInt32 ppwzMimeOut,
            UInt32 dwReserverd
        );

        public static string GetMimeFromFile(this string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName + " not found");
            var buffer = new byte[256];
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                if (fs.Length >= 256)
                    fs.Read(buffer, 0, 256);
                else
                    fs.Read(buffer, 0, (int)fs.Length);
            }
            try
            {
                UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                var mimeTypePtr = new IntPtr(mimetype);
                var mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                return mime;
            }
            catch { return "unknown/unknown"; }
        }
    }
}
