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
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using Newtonsoft.Json;
    using ReactiveUI;

    public class AppViewModel : ReactiveObject
    {
        private readonly object[] pipeline;

        private IList<CompilerError> errors = new CompilerError[0];
        private string fileName = "";
        private bool grammarChanged = false;
        private string grammarFileName = Path.Combine(Environment.CurrentDirectory, "Grammar.peg");
        private string grammarText = string.Join(Environment.NewLine, new[] { "greeting", "  = \"Hello, world!\" EOF", "", "EOF", "  = !.", "" });
        private string testFileName = "Test";
        private string testResults;
        private string testText = "Hello, world!";

        public AppViewModel()
        {
            var grammarNameChanges = this.WhenAny(x => x.GrammarFileName, x => x.Value);
            var grammarTextChanges = this.WhenAny(x => x.GrammarText, x => x.Value);
            var testNameChanges = this.WhenAny(x => x.TestFileName, x => x.Value);
            var testTextChanges = this.WhenAny(x => x.TestText, x => x.Value);

            var pegParser = new Pipeline.PegParser(grammarTextChanges, grammarNameChanges);
            var pegCompiler = new Pipeline.PegCompiler(pegParser.Grammars);
            var csCompiler = new Pipeline.CsCompiler(pegCompiler.Codes.Zip(pegParser.Grammars, Tuple.Create), grammarNameChanges);
            var testParser = new Pipeline.TestParser(csCompiler.Parsers, testTextChanges, testNameChanges);

            testParser.Results.Select(r => JsonConvert.SerializeObject(r, Formatting.Indented)).BindTo(this, x => x.TestResults);

            this.pipeline = new object[] { pegParser, pegCompiler, csCompiler, testParser };
            var errorObvervables = new List<IObservable<IEnumerable<CompilerError>>>
            {
                pegParser.Errors,
                pegCompiler.Errors,
                csCompiler.Errors,
                testParser.Errors,
            };
            errorObvervables.Aggregate((a, b) => a.CombineLatest(b, (e, r) => e.Concat(r))).Select(e => e.ToList()).BindTo(this, x => x.CompileErrors);

            this.Save = new ReactiveCommand();
            this.Save.RegisterAsyncAction(_ =>
            {
                File.WriteAllText(this.grammarFileName, this.grammarText);
                this.GrammarChanged = false;
            });
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

        public bool GrammarChanged
        {
            get { return this.grammarChanged; }
            private set { this.RaiseAndSetIfChanged(ref this.grammarChanged, value); }
        }

        public string GrammarFileName
        {
            get { return this.grammarFileName; }
            set { this.RaiseAndSetIfChanged(ref this.grammarFileName, value); }
        }

        public string GrammarText
        {
            get { return this.grammarText; }
            set
            {
                this.RaiseAndSetIfChanged(ref this.grammarText, value);
                this.GrammarChanged = true;
            }
        }

        public IReactiveCommand Save { get; protected set; }

        public string TestFileName
        {
            get { return this.testFileName; }
            private set { this.RaiseAndSetIfChanged(ref this.testFileName, value); }
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
    }
}
