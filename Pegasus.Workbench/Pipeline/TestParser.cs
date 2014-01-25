// -----------------------------------------------------------------------
// <copyright file="TestParser.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

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

    internal sealed class TestParser : IDisposable
    {
        private readonly IDisposable disposable;

        public TestParser(IObservable<dynamic> parsers, IObservable<string> subjects, IObservable<string> fileNames)
        {
            var testParserResults = subjects
                .CombineLatest(fileNames, (subject, fileName) => new { subject, fileName })
                .CombineLatest(parsers, (p, parser) => new { p.subject, p.fileName, parser = (object)parser })
                .Throttle(TimeSpan.FromMilliseconds(10), Scheduler.Default)
                .Select(p => ParseTest(p.parser, p.subject, p.fileName))
                .Publish();

            this.Results = testParserResults.Select(r => r.Result);
            this.Errors = testParserResults.Select(r => r.Errors.AsReadOnly());

            this.disposable = testParserResults.Connect();
        }

        public IObservable<IList<CompilerError>> Errors { get; set; }

        public IObservable<object> Results { get; set; }

        public void Dispose()
        {
            this.disposable.Dispose();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception that happens during parsing should be reported through the UI.")]
        private static ParseResult ParseTest(dynamic parser, string subject, string fileName)
        {
            if (parser == null)
            {
                return new ParseResult
                {
                    Errors = new List<CompilerError>(),
                    Result = null,
                };
            }

            subject = subject ?? "";

            try
            {
                return new ParseResult
                {
                    Result = parser.Parse(subject, fileName),
                    Errors = new List<CompilerError>(),
                };
            }
            catch (Exception ex)
            {
                var cursor = ex.Data["cursor"] as Cursor ?? new Cursor(subject, 0, fileName);

                var parts = Regex.Split(ex.Message, @"(?<=^\w+):");
                if (parts.Length == 1)
                {
                    parts = new[] { "", parts[0] };
                }

                return new ParseResult
                {
                    Errors = new List<CompilerError>
                    {
                        new CompilerError(fileName, cursor.Line, cursor.Column, errorNumber: parts[0], errorText: parts[1]),
                    },
                };
            }
        }

        private class ParseResult
        {
            public List<CompilerError> Errors { get; set; }

            public object Result { get; set; }
        }
    }
}
