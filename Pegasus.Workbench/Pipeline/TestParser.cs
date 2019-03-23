// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Workbench.Pipeline
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using Pegasus.Common;
    using Pegasus.Common.Tracing;
    using Pegasus.Workbench.Pipeline.Model;

    internal sealed class TestParser
    {
        public const string SentinelFileName = "_.txt";

        public TestParser(IObservable<ParserEntrypoint> entrypoints, IObservable<string> subjects)
        {
            var testParserResults = subjects
                .CombineLatest(entrypoints, (subject, entrypoint) => new { subject, entrypoint })
                .ObserveOn(Scheduler.Default)
                .Select(p => ParseTest(p.entrypoint, p.subject))
                .Publish()
                .RefCount();

            this.Results = testParserResults.Select(r => r.Result);
            this.Errors = testParserResults.Select(r => r.Errors);
        }

        public IObservable<IList<CompilerError>> Errors { get; set; }

        public IObservable<object> Results { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception that happens during parsing should be reported through the UI.")]
        private static ParseResult ParseTest(ParserEntrypoint entrypoint, string subject)
        {
            if (entrypoint == null)
            {
                return new ParseResult
                {
                    Errors = new CompilerError[0],
                    Result = null,
                };
            }

            subject = subject ?? string.Empty;

            try
            {
                var tracer = new WarningsPerformanceTracer();
                var result = entrypoint.Parse(subject, SentinelFileName, tracer);
                return new ParseResult
                {
                    Result = result,
                    Errors = tracer.Recommendations,
                };
            }
            catch (Exception ex)
            {
                var cursor = ex.Data["cursor"] as Cursor ?? new Cursor(subject, 0, SentinelFileName);

                var parts = Regex.Split(ex.Message, @"(?<=^\w+):");
                if (parts.Length == 1)
                {
                    parts = new[] { string.Empty, parts[0] };
                }

                return new ParseResult
                {
                    Errors = new List<CompilerError>
                    {
                        new CompilerError(SentinelFileName, cursor.Line, cursor.Column, errorNumber: parts[0], errorText: parts[1]),
                    }.AsReadOnly(),
                };
            }
        }

        private class ParseResult
        {
            public IList<CompilerError> Errors { get; set; }

            public object Result { get; set; }
        }

        private class WarningsPerformanceTracer : PerformanceTracer
        {
            public IList<CompilerError> Recommendations { get; set; } = new CompilerError[0];

            protected override void ReportPerformance(TimeSpan averageCacheHitDuration, RulePerformanceInfo[] stats)
            {
                this.Recommendations = Enumerable.Concat(
                    stats.Where(stat => !stat.IsCached && stat.EstimatedTotalTimeSaved > TimeSpan.Zero).Select(stat =>
                        new CompilerError(string.Empty, 0, 0, string.Empty, $"Recommendation: Add the -memoize flag to `{stat.Name}`. (Saves {stat.EstimatedTotalTimeSaved})") { IsWarning = true }),
                    stats.Where(stat => stat.IsCached && stat.EstimatedTotalTimeSaved < -TimeSpan.FromMilliseconds(10)).Select(stat =>
                        new CompilerError(string.Empty, 0, 0, string.Empty, $"Recommendation: Remove -memoize flag from `{stat.Name}`. (Saves {stat.EstimatedTotalTimeSaved.Negate()})") { IsWarning = true }))
                    .ToList();
            }
        }
    }
}
