// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Pegasus.Common;
    using Pegasus.Common.Tracing;

    /// <summary>
    /// Measures the performance of rules for the traced grammar.
    /// </summary>
    /// <remarks>
    /// The default implementation of this class uses <see cref="Trace"/> to display the results. Override this class to customize how results are presented.
    /// </remarks>
    public class PerformanceTracer : ITracer
    {
        private RuleStats cacheHitStats = new RuleStats();
        private Stack<RuleStackEntry> ruleStack = new Stack<RuleStackEntry>();
        private Dictionary<string, RuleStats> stats = new Dictionary<string, RuleStats>();

        /// <inheritdoc/>
        public void TraceCacheHit<T>(string ruleName, Cursor cursor, CacheKey cacheKey, IParseResult<T> parseResult)
        {
            this.ruleStack.Peek().CacheHit = true;
            this.stats[ruleName].CacheHits++;
        }

        /// <inheritdoc/>
        public void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey)
        {
            this.ruleStack.Peek().CacheHit = false;
            this.stats[ruleName].CacheMisses++;
        }

        /// <inheritdoc/>
        public void TraceInfo(string ruleName, Cursor cursor, string info)
        {
        }

        /// <inheritdoc/>
        public void TraceRuleEnter(string ruleName, Cursor cursor)
        {
            this.ruleStack.Push(new RuleStackEntry
            {
                RuleName = ruleName,
                Cursor = cursor,
                Stopwatch = Stopwatch.StartNew(),
            });

            RuleStats ruleStats;
            if (!this.stats.TryGetValue(ruleName, out ruleStats))
            {
                ruleStats = this.stats[ruleName] = new RuleStats();
            }

            ruleStats.Invocations++;
            var key = new CacheKey(ruleName, cursor.StateKey, cursor.Location);
            ruleStats.Locations.TryGetValue(key, out var count);
            ruleStats.Locations[key] = count + 1;
        }

        /// <inheritdoc/>
        public void TraceRuleExit<T>(string ruleName, Cursor cursor, IParseResult<T> parseResult)
        {
            var entry = this.ruleStack.Pop();
            entry.Stopwatch.Stop();
            var ticks = entry.Stopwatch.ElapsedTicks;
            if (entry.CacheHit ?? false)
            {
                this.cacheHitStats.Invocations += 1;
                this.cacheHitStats.TotalTicks += ticks;
            }
            else
            {
                this.stats[ruleName].TotalTicks += ticks;
            }

            if (this.ruleStack.Count == 0)
            {
                var cacheHitTicks = this.cacheHitStats.Invocations == 0
                    ? 1.35
                    : (double)this.cacheHitStats.TotalTicks / this.cacheHitStats.Invocations;

                this.ReportPerformance(cacheHitTicks, this.stats.Select(stat =>
                {
                    var stats = stat.Value;
                    var isCached = stats.CacheMisses > 0;
                    var cacheHits = isCached ? stats.CacheHits : stats.Locations.Values.Where(v => v > 1).Select(v => v - 1).Sum();
                    var cacheMisses = isCached ? stats.CacheMisses : stats.Locations.Count;
                    var averageTicks = (double)stats.TotalTicks / (isCached ? stats.CacheMisses : stats.Invocations);

                    var estimatedTimeWithoutCache = (cacheHits + cacheMisses) * averageTicks;
                    var estimatedTimeWithCache = (cacheMisses * averageTicks) + ((cacheHits + cacheMisses) * cacheHitTicks);
                    var estimatedTimeSaved = estimatedTimeWithoutCache - estimatedTimeWithCache;

                    return new RulePerformanceInfo
                    {
                        Name = stat.Key,
                        Invocations = stats.Invocations,
                        AverageTicks = averageTicks,
                        IsCached = isCached,
                        CacheHits = cacheHits,
                        CacheMisses = cacheMisses,
                        EstimatedTotalTicksSaved = estimatedTimeSaved,
                    };
                }).ToArray());
            }
        }

        /// <summary>
        /// Displays or otherwise presents the results of the trace.
        /// </summary>
        /// <param name="averageCacheHitTicks">The average duration of a cache hit.</param>
        /// <param name="stats">The performance stats to report.</param>
        protected virtual void ReportPerformance(double averageCacheHitTicks, RulePerformanceInfo[] stats)
        {
#if !NETSTANDARD1_0

            Trace.WriteLine($"Average Cache Hit Duration: {TimeSpan.FromTicks((long)Math.Round(averageCacheHitTicks))}");
            foreach (var stat in stats)
            {
                Trace.WriteLine($"Rule: {stat.Name}");
                Trace.Indent();

                Trace.WriteLine($"Invocations: {stat.Invocations}");
                Trace.WriteLine($"Average Duration: {TimeSpan.FromTicks((long)Math.Round(stat.AverageTicks))}");
                Trace.WriteLine($"Is Cached: {stat.IsCached}");
                Trace.Indent();

                if (stat.IsCached)
                {
                    Trace.WriteLine($"Cache Hits: {stat.CacheHits}");
                    Trace.WriteLine($"Cache Misses: {stat.CacheMisses}");
                }
                else
                {
                    Trace.WriteLine($"Redundant Invocations: {stat.CacheHits}");
                }

                Trace.Unindent();

                var timeSaved = TimeSpan.FromTicks((long)Math.Round(stat.EstimatedTotalTicksSaved));

                if (stat.IsCached || stat.CacheHits > 0)
                {
                    Trace.WriteLine($"Estimated Time Saved: {timeSaved}");
                }

                if (!stat.IsCached && stat.EstimatedTotalTicksSaved > 0)
                {
                    Trace.WriteLine($"Recommendation: Add the -memoize flag to `{stat.Name}`. (Saves {timeSaved})");
                }
                else if (stat.IsCached && stat.EstimatedTotalTicksSaved < -TimeSpan.FromMilliseconds(10).Ticks)
                {
                    Trace.WriteLine($"Recommendation: Remove -memoize flag from `{stat.Name}`. (Saves {timeSaved.Negate()})");
                }

                Trace.Unindent();
            }

#endif
        }

        /// <summary>
        /// Summarizes the performance of a specific rule.
        /// </summary>
        protected class RulePerformanceInfo
        {
            /// <summary>
            /// Gets the average duration of each invocation.
            /// </summary>
            public double AverageTicks { get; internal set; }

            /// <summary>
            /// Gets the total number of invocations that were a cache hit, or the total number of redundant invocations if the rule is not cached.
            /// </summary>
            public int CacheHits { get; internal set; }

            /// <summary>
            /// Gets the total number of invocations that were a cache miss, or the total number of unique invocations if the rule is not cached.
            /// </summary>
            public int CacheMisses { get; internal set; }

            /// <summary>
            /// Gets the estimated total time saved by memoizing this rule.
            /// </summary>
            public double EstimatedTotalTicksSaved { get; internal set; }

            /// <summary>
            /// The total number of invocations.
            /// </summary>
            public int Invocations { get; internal set; }

            /// <summary>
            /// Gets a value indicating whether or not this rule is memoized.
            /// </summary>
            public bool IsCached { get; internal set; }

            /// <summary>
            /// Gets the name of the rule.
            /// </summary>
            public string Name { get; internal set; }
        }

        private class RuleStackEntry
        {
            public bool? CacheHit { get; set; }

            public Cursor Cursor { get; set; }

            public string RuleName { get; set; }

            public Stopwatch Stopwatch { get; set; }
        }

        private class RuleStats
        {
            public int CacheHits { get; set; }

            public int CacheMisses { get; set; }

            public int Invocations { get; set; }

            public Dictionary<CacheKey, int> Locations { get; } = new Dictionary<CacheKey, int>();

            public long TotalTicks { get; set; }
        }
    }
}
