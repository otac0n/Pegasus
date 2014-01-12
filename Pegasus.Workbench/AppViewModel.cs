// -----------------------------------------------------------------------
// <copyright file="AppViewModel.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Microsoft.CSharp;
    using Newtonsoft.Json;
    using Pegasus.Common;
    using Pegasus.Compiler;
    using Pegasus.Expressions;
    using Pegasus.Parser;
    using ReactiveUI;

    public class AppViewModel : ReactiveObject
    {
        private IList<CompilerError> errors = new CompilerError[0];
        private string fileName = "";
        private string grammarText;
        private string testResults;
        private string testText;

        public AppViewModel()
        {
            var grammarTextChanges = this.WhenAny(x => x.GrammarText, x => x.Value).ObserveOn(Scheduler.Default);
            var testTextChanges = this.WhenAny(x => x.TestText, x => x.Value).ObserveOn(Scheduler.Default);

            var pegParserResults = grammarTextChanges.Select(g => Parse(g, "Grammar"));
            var pegCompilerResults = pegParserResults.Select(r => r.Grammar == null
                                         ? new CompileResult(r.Grammar)
                                         : PegCompiler.Compile(r.Grammar));
            var csCompilerResults = pegCompilerResults.Select(r => r.Code == null
                                        ? new CompilerResults(new TempFileCollection()) { NativeCompilerReturnValue = -1 }
                                        : Compile(r.Code));

            var parsers = pegParserResults.Zip(csCompilerResults, (p, c) => p.Grammar == null || c.NativeCompilerReturnValue != 0 ? null : GetParser(p.Grammar, c.CompiledAssembly));

            var testParserResults = parsers.CombineLatest(testTextChanges, (p, t) => p == null ? new ParseTestResult { Errors = new CompilerError[0] } : ParseTest((object)p, t, "Test"));
            testParserResults.Select(r => JsonConvert.SerializeObject(r.Result, Formatting.Indented)).BindTo(this, x => x.TestResults);

            var errors = pegParserResults.Select(r => r.Errors.AsEnumerable());
            errors = errors.CombineLatest(pegCompilerResults, (e, r) => e.Concat(r.Errors));
            errors = errors.CombineLatest(csCompilerResults, (e, r) => e.Concat(r.Errors.Cast<CompilerError>()));
            errors = errors.CombineLatest(testParserResults, (e, r) => e.Concat(r.Errors));
            errors.Select(Enumerable.ToList).BindTo(this, x => x.CompileErrors);
        }

        public IList<CompilerError> CompileErrors
        {
            get { return this.errors; }
            protected set { this.RaiseAndSetIfChanged(ref this.errors, value); }
        }

        public string FileName
        {
            get { return this.fileName; }
            set { this.RaiseAndSetIfChanged(ref this.fileName, value); }
        }

        public string GrammarText
        {
            get { return this.grammarText; }
            set { this.RaiseAndSetIfChanged(ref this.grammarText, value); }
        }

        public string TestResults
        {
            get { return this.testResults; }
            private set { this.RaiseAndSetIfChanged(ref this.testResults, value); }
        }

        public string TestText
        {
            get { return this.testText; }
            set { this.RaiseAndSetIfChanged(ref this.testText, value); }
        }

        private static CompilerResults Compile(string source)
        {
            var compiler = new CSharpCodeProvider();
            var options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add(typeof(Cursor).Assembly.Location);

            return compiler.CompileAssemblyFromSource(options, source);
        }

        private static ParseResult Parse(string subject, string fileName)
        {
            try
            {
                return new ParseResult
                {
                    Grammar = new PegParser().Parse(subject ?? "", fileName),
                    Errors = new CompilerError[0],
                };
            }
            catch (FormatException ex)
            {
                var cursor = ex.Data["cursor"] as Cursor;
                if (cursor != null)
                {
                    var parts = Regex.Split(ex.Message, @"(?<=^\w+):");
                    if (parts.Length == 1)
                    {
                        parts = new[] { "", parts[0] };
                    }

                    return new ParseResult
                    {
                        Errors = new[]
                        {
                            new CompilerError(cursor.FileName, cursor.Line, cursor.Column, parts[0], parts[1]),
                        },
                    };
                }

                throw;
            }
        }

        private static ParseTestResult ParseTest(dynamic parser, string subject, string fileName)
        {
            try
            {
                return new ParseTestResult
                {
                    Result = parser.Parse(subject ?? "", fileName),
                    Errors = new CompilerError[0],
                };
            }
            catch (FormatException ex)
            {
                var cursor = ex.Data["cursor"] as Cursor;
                if (cursor != null)
                {
                    var parts = Regex.Split(ex.Message, @"(?<=^\w+):");
                    if (parts.Length == 1)
                    {
                        parts = new[] { "", parts[0] };
                    }

                    return new ParseTestResult
                    {
                        Errors = new[]
                        {
                            new CompilerError(cursor.FileName, cursor.Line, cursor.Column, parts[0], parts[1]),
                        },
                    };
                }

                throw;
            }
        }

        private dynamic GetParser(Grammar grammar, Assembly assembly)
        {
            var @namespace = grammar.Settings.Where(s => s.Key.Name == "namespace").Select(s => s.Value.ToString()).SingleOrDefault() ?? "Parsers";
            var @className = grammar.Settings.Where(s => s.Key.Name == "classname").Select(s => s.Value.ToString()).SingleOrDefault() ?? "Parser";

            try
            {
                return Activator.CreateInstance(assembly.GetType(@namespace + "." + @className));
            }
            catch
            {
                return null;
            }
        }

        private class ParseResult
        {
            public IList<CompilerError> Errors { get; set; }

            public Grammar Grammar { get; set; }
        }

        private class ParseTestResult
        {
            public IList<CompilerError> Errors { get; set; }

            public object Result { get; set; }
        }
    }
}
