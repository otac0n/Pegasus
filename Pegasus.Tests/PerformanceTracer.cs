// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Pegasus.Common;
    using Pegasus.Common.Tracing;

    public class PerformanceTracer : ITracer
    {
        private RuleStats cacheHitStats = new RuleStats();
        private Dictionary<string, RuleStats> stats = new Dictionary<string, RuleStats>();
        private Stack<RuleStackEntry> ruleStack = new Stack<RuleStackEntry>();

        public void TraceCacheHit<T>(string ruleName, Cursor cursor, CacheKey cacheKey, IParseResult<T> parseResult)
        {
            this.ruleStack.Peek().CacheHit = true;
            this.stats[ruleName].CacheHits++;
        }

        public void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey)
        {
            this.ruleStack.Peek().CacheHit = false;
            this.stats[ruleName].CacheMisses++;
        }

        public void TraceInfo(string ruleName, Cursor cursor, string info)
        {
        }

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
            ruleStats.Locations.TryGetValue(cursor.Location, out var count);
            ruleStats.Locations[cursor.Location] = count + 1;
        }

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

                Trace.WriteLine($"Average Cache Hit Duration: {TimeSpan.FromTicks((long)Math.Round(cacheHitTicks))}");

                foreach (var stat in this.stats)
                {
                    var stats = stat.Value;
                    var isCached = stats.CacheMisses > 0;
                    var cacheHits = isCached ? stats.CacheHits : stats.Locations.Values.Where(v => v > 1).Select(v => v - 1).Sum();
                    var cacheMisses = isCached ? stats.CacheMisses : stats.Locations.Count;
                    var averageTicks = (double)stats.TotalTicks / (isCached ? stats.CacheMisses : stats.Invocations);

                    Trace.WriteLine($"Rule: {stat.Key}");
                    Trace.Indent();

                    Trace.WriteLine($"Invocations: {stats.Invocations}");
                    Trace.WriteLine($"Average Duration: {TimeSpan.FromTicks((long)Math.Round(averageTicks))}");
                    Trace.WriteLine($"Is Cached: {isCached}");
                    Trace.Indent();

                    if (isCached)
                    {
                        Trace.WriteLine($"Cache Hits: {cacheHits}");
                        Trace.WriteLine($"Cache Misses: {cacheMisses}");
                    }
                    else
                    {
                        Trace.WriteLine($"Redundant Invocations: {cacheHits}");
                    }

                    Trace.Unindent();

                    var estimatedTimeWithoutCache = (cacheHits + cacheMisses) * averageTicks;
                    var estimatedTimeWithCache = cacheMisses * averageTicks + (cacheHits + cacheMisses) * cacheHitTicks;
                    var estimatedTimeSaved = estimatedTimeWithoutCache - estimatedTimeWithCache;
                    var timeSaved = TimeSpan.FromTicks((long)Math.Round(estimatedTimeSaved));
                    if (isCached || cacheHits > 0)
                    {
                        Trace.WriteLine($"Estimated Time Saved: {timeSaved}");
                    }

                    if (!isCached && estimatedTimeSaved > 0)
                    {
                        Trace.WriteLine($"Recommendation: Add the -memoize flag to `{stat.Key}`. (Saves {timeSaved})");
                    }
                    else if (isCached && estimatedTimeSaved < -TimeSpan.FromMilliseconds(10).Ticks)
                    {
                        Trace.WriteLine($"Recommendation: Remove -memoize flag from `{stat.Key}`. (Saves {timeSaved.Negate()})");
                    }

                    Trace.Unindent();
                }
            }
        }

        private class RuleStackEntry
        {
            public string RuleName { get; set; }

            public Cursor Cursor { get; set; }

            public bool? CacheHit { get; set; }

            public Stopwatch Stopwatch { get; set; }
        }

        private class RuleStats
        {
            public int Invocations { get; set; }

            public int CacheHits { get; set; }

            public int CacheMisses { get; set; }

            public long TotalTicks { get; set; }

            public Dictionary<int, int> Locations { get; } = new Dictionary<int, int>();
        }
    }
}
