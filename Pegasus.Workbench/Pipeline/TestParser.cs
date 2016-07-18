// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Workbench.Pipeline
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using Pegasus.Common;

    internal sealed class TestParser
    {
        public const string SentinelFileName = "_.txt";

        public TestParser(IObservable<dynamic> parsers, IObservable<string> subjects)
        {
            var testParserResults = subjects
                .CombineLatest(parsers, (subject, parser) => new { subject, parser = (object)parser })
                .ObserveOn(Scheduler.Default)
                .Select(p => ParseTest(p.parser, p.subject))
                .Publish()
                .RefCount();

            this.Results = testParserResults.Select(r => r.Result);
            this.Errors = testParserResults.Select(r => r.Errors);
        }

        public IObservable<IList<CompilerError>> Errors { get; set; }

        public IObservable<object> Results { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception that happens during parsing should be reported through the UI.")]
        private static ParseResult ParseTest(dynamic parser, string subject)
        {
            if (parser == null)
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
                return new ParseResult
                {
                    Result = parser.Parse(subject, SentinelFileName),
                    Errors = new CompilerError[0],
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
    }
}
