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
    using Pegasus.Expressions;

    internal sealed class PegParser
    {
        public const string SentinelFileName = "_.peg";

        public PegParser(IObservable<string> subjects)
        {
            var internalParseResults = subjects
                .ObserveOn(Scheduler.Default)
                .Select(Parse)
                .Publish()
                .RefCount();

            this.Grammars = internalParseResults.Select(g => g.Grammar);
            this.Errors = internalParseResults.Select(g => g.Errors.AsReadOnly());
            this.LexicalElements = internalParseResults.Select(g => g.LexicalElements);
        }

        public IObservable<IList<CompilerError>> Errors { get; }

        public IObservable<Grammar> Grammars { get; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "TODO: Use this for syntax highlighting.")]
        public IObservable<IList<LexicalElement>> LexicalElements { get; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception that happens during parsing should be reported through the UI.")]
        private static ParseResult Parse(string subject)
        {
            subject = subject ?? string.Empty;

            IList<LexicalElement> lexicalElements;
            try
            {
                var grammar = new Pegasus.Parser.PegParser().Parse(subject, SentinelFileName, out lexicalElements);

                return new ParseResult
                {
                    Grammar = grammar,
                    LexicalElements = lexicalElements,
                    Errors = new List<CompilerError>(),
                };
            }
            catch (Exception ex)
            {
                var cursor = ex.Data["cursor"] as Cursor ?? new Cursor(subject, 0, SentinelFileName);
                lexicalElements = cursor.GetLexicalElements();

                var parts = Regex.Split(ex.Message, @"(?<=^\w+):");
                if (parts.Length == 1)
                {
                    parts = new[] { string.Empty, parts[0] };
                }

                return new ParseResult
                {
                    LexicalElements = lexicalElements,
                    Errors = new List<CompilerError>
                    {
                        new CompilerError(SentinelFileName, cursor.Line, cursor.Column, errorNumber: parts[0], errorText: parts[1]),
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
