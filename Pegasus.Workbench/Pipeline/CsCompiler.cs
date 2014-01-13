namespace Pegasus.Workbench.Pipeline
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Microsoft.CSharp;
    using Pegasus.Common;
    using Pegasus.Expressions;

    public class CsCompiler
    {
        public CsCompiler(IObservable<Tuple<string, Grammar>> codeAndGrammar, IObservable<string> fileNames)
        {
            var csCompilerResults = codeAndGrammar
                .CombineLatest(fileNames, (cg, fileName) => new { code = cg.Item1, grammar = cg.Item2, fileName })
                .ObserveOn(Scheduler.Default)
                .Throttle(TimeSpan.FromMilliseconds(10))
                .Select(p => Compile(p.code, p.grammar, p.fileName));

            this.Parsers = csCompilerResults.Select(r => r.Parser);
            this.Errors = csCompilerResults.Select(r => r.Errors.AsReadOnly());
        }

        public IObservable<IList<CompilerError>> Errors { get; private set; }

        public IObservable<dynamic> Parsers { get; private set; }

        private static Result Compile(string source, Grammar grammar, string fileName)
        {
            if (source == null || grammar == null)
            {
                return new Result
                {
                    Errors = new List<CompilerError>(),
                };
            }

            var compiler = new CSharpCodeProvider();
            var options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add(typeof(Cursor).Assembly.Location);

            var compilerResults = compiler.CompileAssemblyFromSource(options, source);
            var errors = compilerResults.Errors.Cast<CompilerError>().ToList();

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
