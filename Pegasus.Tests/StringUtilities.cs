// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static class StringUtilities
    {
        public static string JoinLines(params string[] lines)
        {
            return JoinLines(lines.AsEnumerable());
        }

        public static string JoinLines(IEnumerable<string> lines)
        {
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        public static string[] SplitLines(this string lines) => lines.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        public static string GetResourceString(this Assembly assembly, string resourceName)
        {
            var fullName = $"{assembly.GetName().Name}.{resourceName}";
            using (var stream = assembly.GetManifestResourceStream(fullName))
            using (var reader = new StreamReader(stream, Encoding.Default, true, 1024, true))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
