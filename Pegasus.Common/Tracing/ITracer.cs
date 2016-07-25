// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Common.Tracing
{
    /// <summary>
    /// Provides an interface for PEG grammars to trace their activity.
    /// </summary>
    /// <remarks>
    /// This is primarily intended for debugging.
    /// </remarks>
    public interface ITracer
    {
        /// <summary>
        /// Signifies that the parser has found a parse result in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="ruleName">The name of the rule being parsed.</param>
        /// <param name="cursor">The cursor where the cache was checked.</param>
        /// <param name="cacheKey">The cache key used.</param>
        /// <param name="parseResult">The result found in the cache.</param>
        void TraceCacheHit<T>(string ruleName, Cursor cursor, CacheKey cacheKey, IParseResult<T> parseResult);

        /// <summary>
        /// Signifies that the parser has not found a parse result in the cache.
        /// </summary>
        /// <param name="ruleName">The name of the rule being parsed.</param>
        /// <param name="cursor">The cursor where the cache was checked.</param>
        /// <param name="cacheKey">The cache key used.</param>
        void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey);

        /// <summary>
        /// Allows the parser to trace additional info relevant to the parse.
        /// </summary>
        /// <param name="ruleName">The name of the rule being parsed.</param>
        /// <param name="cursor">The cursor where the info is presented.</param>
        /// <param name="info">The info being included.</param>
        void TraceInfo(string ruleName, Cursor cursor, string info);

        /// <summary>
        /// Signifies that the parser has began parsing the specified rule.
        /// </summary>
        /// <param name="ruleName">The name of the rule being parsed.</param>
        /// <param name="cursor">The cursor where ther parse will be attempted.</param>
        void TraceRuleEnter(string ruleName, Cursor cursor);

        /// <summary>
        /// Signifies that the parser has finished parsing the specified rule.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="ruleName">The name of the rule being parsed.</param>
        /// <param name="cursor">The cursor where this rule has left off.</param>
        /// <param name="parseResult">The result of the parse.</param>
        void TraceRuleExit<T>(string ruleName, Cursor cursor, IParseResult<T> parseResult);
    }
}
