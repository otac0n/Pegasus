// -----------------------------------------------------------------------
// <copyright file="CsCompiler.cs" company="(none)">
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
    using System.IO;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Microsoft.CSharp;
    using Pegasus.Common;
    using Pegasus.Expressions;

    internal sealed class CsCompiler : IDisposable
    {
        private readonly IDisposable disposable;

        public CsCompiler(IObservable<Tuple<string, Grammar>> codeAndGrammar, IObservable<string> fileNames)
        {
            var csCompilerResults = codeAndGrammar
                .CombineLatest(fileNames, (cg, fileName) => new { code = cg.Item1, grammar = cg.Item2, fileName })
                .Throttle(TimeSpan.FromMilliseconds(10), Scheduler.Default)
                .Select(p => Compile(p.code, p.grammar, p.fileName))
                .Publish();

            this.Parsers = csCompilerResults.Select(r => r.Parser);
            this.Errors = csCompilerResults.Select(r => r.Errors.AsReadOnly());

            this.disposable = csCompilerResults.Connect();
        }

        public IObservable<IList<CompilerError>> Errors { get; private set; }

        public IObservable<dynamic> Parsers { get; private set; }

        public void Dispose()
        {
            this.disposable.Dispose();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception that happens during compilation should be reported through the UI.")]
        private static Result Compile(string source, Grammar grammar, string fileName)
        {
            if (source == null || grammar == null)
            {
                return new Result
                {
                    Errors = new List<CompilerError>(),
                };
            }

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
                options.ReferencedAssemblies.Add(typeof(Cursor).Assembly.Location);

                compilerResults = compiler.CompileAssemblyFromSource(options, source);
            }

            var fileDirectory = Path.GetDirectoryName(fileName);
            var fileFileName = Path.GetFileName(fileName);

            var errors = (from CompilerError e in compilerResults.Errors
                          let errorFileName = Path.GetFileName(e.FileName)
                          let newName = errorFileName.Equals(fileFileName, StringComparison.CurrentCultureIgnoreCase) ? fileFileName : fileFileName + ".cs"
                          let newPath = Path.Combine(fileDirectory, newName)
                          select new CompilerError(newPath, e.Line, e.Column, e.ErrorNumber, e.ErrorText) { IsWarning = e.IsWarning }).ToList();

            if (errors.Any(e => !e.IsWarning))
            {
                return new Result
                {
                    Errors = errors,
                };
            }

            var @namespace = grammar.Settings.Where(s => s.Key.Name == "namespace").Select(s => s.Value.ToString()).SingleOrDefault() ?? "Parsers";
            var @className = grammar.Settings.Where(s => s.Key.Name == "classname").Select(s => s.Value.ToString()).SingleOrDefault() ?? "Parser";

            try
            {
                var parserType = compilerResults.CompiledAssembly.GetType(@namespace + "." + @className);
                return new Result
                {
                    Parser = Activator.CreateInstance(parserType),
                    Errors = errors,
                };
            }
            catch (Exception ex)
            {
                errors.Add(new CompilerError(fileName, 1, 1, "", "Internal Error: " + ex.Message));
                return new Result
                {
                    Errors = errors,
                };
            }
        }

        private class Result
        {
            public List<CompilerError> Errors { get; set; }

            public dynamic Parser { get; set; }
        }
    }
}
