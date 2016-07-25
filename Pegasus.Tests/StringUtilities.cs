// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System.Collections.Generic;
    using System.Linq;
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
    }
}
