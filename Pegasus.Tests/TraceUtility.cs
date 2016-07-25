// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal static class TraceUtility
    {
        public static string Trace(Action body)
        {
            var writer = new StringWriter();
            var listener = new TextWriterTraceListener(writer);

            try
            {
                System.Diagnostics.Trace.Listeners.Add(listener);
                body();
            }
            finally
            {
                System.Diagnostics.Trace.Listeners.Remove(listener);
            }

            return writer.ToString();
        }
    }
}
