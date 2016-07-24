// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Common.Tracing
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Implements an <see cref="ITracer"/> that writes to <see cref="Trace"/>.
    /// </summary>
    public class DiagnosticsTracer : ITracer
    {
        private DiagnosticsTracer()
        {
        }

        /// <summary>
        /// Gets the instance of <see cref="NullTracer"/>.
        /// </summary>
        public static DiagnosticsTracer Instance { get; } = new DiagnosticsTracer();

        /// <inheritdoc/>
        public void TraceCacheHit<T>(string ruleName, Cursor cursor, CacheKey cacheKey, IParseResult<T> parseResult)
        {
            this.TraceInfo(ruleName, cursor, "Cache hit.");
        }

        /// <inheritdoc/>
        public void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey)
        {
            this.TraceInfo(ruleName, cursor, "Cache miss.");
        }

        /// <inheritdoc/>
        public void TraceInfo(string ruleName, Cursor cursor, string info)
        {
            Trace.WriteLine(info);
        }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validation excluded for performance reasons.")]
        public void TraceRuleEnter(string ruleName, Cursor cursor)
        {
            Trace.WriteLine($"Begin '{ruleName}' at ({cursor.Line},{cursor.Column}) with state key {cursor.StateKey}");
            Trace.Indent();
        }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validation excluded for performance reasons.")]
        public void TraceRuleExit<T>(string ruleName, Cursor cursor, IParseResult<T> parseResult)
        {
            var success = parseResult != null;
            Trace.Unindent();
            Trace.WriteLine($"End '{ruleName}' with {(success ? "success" : "failure")} at ({cursor.Line},{cursor.Column}) with state key {cursor.StateKey}");
        }
    }
}
