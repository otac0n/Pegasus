// -----------------------------------------------------------------------
// <copyright file="AppViewModel.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Newtonsoft.Json;
    using ReactiveUI;

    /// <summary>
    /// Controls the interaction of the app.
    /// </summary>
    public sealed class AppViewModel : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable pipeline;

        private IList<CompilerError> errors = new CompilerError[0];
        private bool grammarChanged = false;
        private string grammarFileName = "Untitled.peg";
        private string grammarText = "";
        private string testFileName = "Test";
        private string testResults;
        private string testText = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppViewModel"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The reactive nature of this object leads to many intermediary types.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The pipeline is disposed properly.")]
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
            this.pipeline = new CompositeDisposable(pegParser, pegCompiler, csCompiler, testParser);

            testParser.Results.Select(r =>
            {
                var s = r as string;
                return s != null ? s : JsonConvert.SerializeObject(r, Formatting.Indented);
            }).BindTo(this, x => x.TestResults);

            var errorObvervables = new List<IObservable<IEnumerable<CompilerError>>>
            {
                pegParser.Errors,
                pegCompiler.Errors,
                csCompiler.Errors,
                testParser.Errors,
            };
            errorObvervables.Aggregate((a, b) => a.CombineLatest(b, (e, r) => e.Concat(r))).Select(e => e.ToList()).BindTo(this, x => x.CompileErrors);

            this.Save = new ReactiveCommand(grammarNameChanges.Select(n => n != "Untitled.peg"));
            this.Save.RegisterAsyncAction(_ =>
            {
                File.WriteAllText(this.grammarFileName, this.grammarText);
                this.GrammarChanged = false;
            });

            this.SaveAs = new ReactiveCommand();
            this.SaveAs.RegisterAsyncAction(_ =>
            {
                var fileName = (string)_;
                File.WriteAllText(fileName, this.grammarText);
                this.GrammarFileName = fileName;
                this.GrammarChanged = false;
            });

            this.Load = new ReactiveCommand();
            this.Load.RegisterAsyncAction(_ =>
            {
                var fileName = (string)_;
                this.GrammarText = File.ReadAllText(fileName);
                this.GrammarFileName = fileName;
                this.GrammarChanged = false;
                this.TestText = "";
            });
        }

        /// <summary>
        /// Gets the list of compile errors.
        /// </summary>
        public IList<CompilerError> CompileErrors
        {
            get { return this.errors; }
            private set { this.RaiseAndSetIfChanged(ref this.errors, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the grammar has changed.
        /// </summary>
        public bool GrammarChanged
        {
            get { return this.grammarChanged; }
            private set { this.RaiseAndSetIfChanged(ref this.grammarChanged, value); }
        }

        /// <summary>
        /// Gets or sets the name of the grammar file.
        /// </summary>
        public string GrammarFileName
        {
            get { return this.grammarFileName; }
            set { this.RaiseAndSetIfChanged(ref this.grammarFileName, value); }
        }

        /// <summary>
        /// Gets or sets the grammar text.
        /// </summary>
        public string GrammarText
        {
            get
            {
                return this.grammarText;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.grammarText, value);
                this.GrammarChanged = true;
            }
        }

        /// <summary>
        /// Gets the load command.
        /// </summary>
        public IReactiveCommand Load { get; private set; }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        public IReactiveCommand Save { get; private set; }

        /// <summary>
        /// Gets the save-as command.
        /// </summary>
        public IReactiveCommand SaveAs { get; private set; }

        /// <summary>
        /// Gets the name of the test file.
        /// </summary>
        public string TestFileName
        {
            get { return this.testFileName; }
            private set { this.RaiseAndSetIfChanged(ref this.testFileName, value); }
        }

        /// <summary>
        /// Gets the test results.
        /// </summary>
        public string TestResults
        {
            get { return this.testResults; }
            private set { this.RaiseAndSetIfChanged(ref this.testResults, value); }
        }

        /// <summary>
        /// Gets or sets the test text.
        /// </summary>
        public string TestText
        {
            get { return this.testText; }
            set { this.RaiseAndSetIfChanged(ref this.testText, value); }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            this.pipeline.Dispose();
        }
    }
}
