// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Workbench.Pipeline
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Microsoft.CSharp;
    using Pegasus.Common;
    using Pegasus.Compiler;
    using Pegasus.Expressions;

    internal sealed class CsCompiler
    {
        public const string SentinelFileName = "_.peg.g.cs";

        public CsCompiler(IObservable<Tuple<string, Grammar>> codeAndGrammar)
        {
            var csCompilerResults = codeAndGrammar
                .ObserveOn(Scheduler.Default)
                .Select(p => Compile(p.Item1, p.Item2))
                .Publish()
                .RefCount();

            this.Parsers = csCompilerResults.Select(r => r.Parser);
            this.Errors = csCompilerResults.Select(r => r.Errors);
        }

        public IObservable<IList<CompilerError>> Errors { get; }

        public IObservable<dynamic> Parsers { get; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception that happens during compilation should be reported through the UI.")]
        private static Result Compile(string source, Grammar grammar)
        {
            if (source == null || grammar == null)
            {
                return new Result
                {
                    Errors = new CompilerError[0],
                };
            }

            var errors = new List<CompilerError>();
            try
            {
                CompilerResults compilerResults;
                using (var compiler = new CSharpCodeProvider())
                {
                    var options = new CompilerParameters
                    {
                        GenerateExecutable = false,
                        GenerateInMemory = true,
                    };
                    options.ReferencedAssemblies.Add("System.dll");
                    options.ReferencedAssemblies.Add("System.Core.dll");
                    options.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                    options.ReferencedAssemblies.Add(typeof(Cursor).Assembly.Location);

                    compilerResults = compiler.CompileAssemblyFromSource(options, source);
                }

                errors.AddRange(from CompilerError e in compilerResults.Errors
                                let fileName = Path.GetFileName(e.FileName)
                                let newName = fileName == PegParser.SentinelFileName ? fileName : SentinelFileName
                                select new CompilerError(newName, e.Line, e.Column, e.ErrorNumber, e.ErrorText) { IsWarning = e.IsWarning });

                if (errors.Any(e => !e.IsWarning))
                {
                    return new Result
                    {
                        Errors = errors.AsReadOnly(),
                    };
                }

                var @namespace = grammar.Settings.Where(s => s.Key.Name == SettingName.Namespace).Select(s => s.Value.ToString()).SingleOrDefault() ?? "Parsers";
                var @className = grammar.Settings.Where(s => s.Key.Name == SettingName.ClassName).Select(s => s.Value.ToString()).SingleOrDefault() ?? "Parser";

                var parserType = compilerResults.CompiledAssembly.GetType(@namespace + "." + @className);
                return new Result
                {
                    Parser = Activator.CreateInstance(parserType),
                    Errors = errors.AsReadOnly(),
                };
            }
            catch (Exception ex)
            {
                errors.Add(new CompilerError(SentinelFileName, 1, 1, string.Empty, "Internal Error: " + ex.Message));
                return new Result
                {
                    Errors = errors.AsReadOnly(),
                };
            }
        }

        private class Result
        {
            public IList<CompilerError> Errors { get; set; }

            public dynamic Parser { get; set; }
        }
    }
}
