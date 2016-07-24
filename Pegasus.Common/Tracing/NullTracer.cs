// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Common.Tracing
{
    /// <summary>
    /// Implements a <see cref="ITracer"/> that does nothing.
    /// </summary>
    public class NullTracer : ITracer
    {
        private NullTracer()
        {
        }

        /// <summary>
        /// Gets the instance of <see cref="NullTracer"/>.
        /// </summary>
        public static NullTracer Instance { get; } = new NullTracer();

        /// <inheritdoc/>
        public void TraceCacheHit<T>(string ruleName, Cursor cursor, CacheKey cacheKey, IParseResult<T> parseResult)
        {
        }

        /// <inheritdoc/>
        public void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey)
        {
        }

        /// <inheritdoc/>
        public void TraceInfo(string ruleName, Cursor cursor, string info)
        {
        }

        /// <inheritdoc/>
        public void TraceRuleEnter(string ruleName, Cursor cursor)
        {
        }

        /// <inheritdoc/>
        public void TraceRuleExit<T>(string ruleName, Cursor cursor, IParseResult<T> parseResult)
        {
        }
    }
}
