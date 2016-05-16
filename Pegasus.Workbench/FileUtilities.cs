// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Workbench
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public static class FileUtilities
    {
        private const int BufferSize = 4096;

        public static async Task WriteAllTextAsync(string path, string contents)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: BufferSize, useAsync: true))
            {
                var encoded = Encoding.Default.GetBytes(contents);
                await stream.WriteAsync(encoded, 0, encoded.Length);
            }
        }

        public static async Task<string> ReadAllTextAsync(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: BufferSize, useAsync: true))
            using (var reader = new StreamReader(stream, Encoding.Default, detectEncodingFromByteOrderMarks: true, bufferSize: BufferSize, leaveOpen: true))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
