// -----------------------------------------------------------------------
// <copyright file="PegParser.cs" company="(none)">
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
    using Pegasus.Expressions;

    internal class PegParser
    {
        public PegParser(IObservable<string> subjects, IObservable<string> fileNames)
        {
            var internalParseResults = subjects.CombineLatest(fileNames, (subject, filename) => new { subject, filename })
                .ObserveOn(Scheduler.Default)
                .Throttle(TimeSpan.FromMilliseconds(10))
                .Select(p => Parse(p.subject, p.filename));

            this.Grammars = internalParseResults.Select(g => g.Grammar);
            this.Errors = internalParseResults.Select(g => g.Errors.AsReadOnly());
            this.LexicalElements = internalParseResults.Select(g => g.LexicalElements);
        }

        public IObservable<IList<CompilerError>> Errors { get; private set; }

        public IObservable<Grammar> Grammars { get; private set; }

        public IObservable<IList<LexicalElement>> LexicalElements { get; private set; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception that happens during parsing should be reported through the UI.")]
        private static ParseResult Parse(string subject, string fileName)
        {
            subject = subject ?? "";

            try
            {
                IList<LexicalElement> lexicalElements;
                var grammar = new Pegasus.Parser.PegParser().Parse(subject, fileName, out lexicalElements);

                return new ParseResult
                {
                    Grammar = grammar,
                    LexicalElements = lexicalElements,
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

            public Grammar Grammar { get; set; }

            public IList<LexicalElement> LexicalElements { get; set; }
        }
    }
}
