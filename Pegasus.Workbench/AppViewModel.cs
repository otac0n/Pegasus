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
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.CSharp;
    using Pegasus.Common;
    using Pegasus.Compiler;
    using Pegasus.Expressions;
    using Pegasus.Parser;
    using ReactiveUI;

    public class AppViewModel : ReactiveObject
    {
        private IList<CompilerError> errors = new CompilerError[0];
        private string fileName = "";
        private string text;

        public AppViewModel()
        {
            var grammarTextChanges = this.WhenAny(x => x.Text, x => x.Value);

            var pegParserResults = from grammar in grammarTextChanges
                                   select Parse(grammar, "Grammar");

            var pegCompilerResults = from r in pegParserResults
                                     select r.Grammar == null
                                         ? new CompileResult(r.Grammar)
                                         : PegCompiler.Compile(r.Grammar);

            var csCompilerResults = from r in pegCompilerResults
                                    select r.Code == null
                                        ? new CompilerResults(new TempFileCollection())
                                        : Compile(r.Code);

            var errors = pegParserResults.Select(r => r.Errors.AsEnumerable());
            errors = errors.CombineLatest(pegCompilerResults, (e, r) => e.Concat(r.Errors));
            errors = errors.CombineLatest(csCompilerResults, (e, r) => e.Concat(r.Errors.Cast<CompilerError>()));
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

        public string Text
        {
            get { return this.text; }
            set { this.RaiseAndSetIfChanged(ref this.text, value); }
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
                if (cursor != null && Regex.IsMatch(ex.Message, @"^PEG\d+:"))
                {
                    var parts = ex.Message.Split(new[] { ':' }, 2);
                    return new ParseResult
                    {
                        Errors = new[]
                        {
                            new CompilerError(cursor.FileName, cursor.Line, cursor.Column, parts[0], parts[1])
                        },
                    };
                }

                throw;
            }
        }

        private class ParseResult
        {
            public IList<CompilerError> Errors { get; set; }

            public Grammar Grammar { get; set; }
        }
    }
}
