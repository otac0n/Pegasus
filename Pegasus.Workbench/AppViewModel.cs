// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

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
        private string grammarText = string.Empty;
        private string testFileName = "Test";
        private string testResults;
        private string testText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppViewModel"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The reactive nature of this object causes the code analyzer to assume more complexity than actually exists.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The reactive nature of this object leads to many intermediary types.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The pipeline is disposed properly.")]
        public AppViewModel()
        {
            this.Tutorials = Tutorial.FindAll();

            var grammarTextChanges = this.WhenAny(x => x.GrammarText, grammarText => grammarText.GetValue());
            var testTextChanges = this.WhenAny(x => x.TestText, testText => testText.GetValue());

            var pegParser = new Pipeline.PegParser(grammarTextChanges);
            var pegCompiler = new Pipeline.PegCompiler(pegParser.Grammars);
            var csCompiler = new Pipeline.CsCompiler(pegCompiler.Codes.Zip(pegParser.Grammars, Tuple.Create));
            var testParser = new Pipeline.TestParser(csCompiler.Parsers, testTextChanges);
            var testResults = testParser.Results.Select(r =>
            {
                return r is string s ? s : JsonConvert.SerializeObject(r, Formatting.Indented);
            });

            var comparer = new CompilerErrorListEqualityComparer();
            var allErrors = Observable.CombineLatest(
                pegParser.Errors.DistinctUntilChanged(comparer),
                pegCompiler.Errors.DistinctUntilChanged(comparer),
                csCompiler.Errors.DistinctUntilChanged(comparer),
                testParser.Errors.DistinctUntilChanged(comparer))
                .Select(errorLists => errorLists.SelectMany(e => e));

            var grammarNameChanges = this.WhenAny(x => x.GrammarFileName, grammarFileName => grammarFileName.GetValue());
            var testNameChanges = this.WhenAny(x => x.TestFileName, testFileName => testFileName.GetValue());

            var compileErrors = Observable.CombineLatest(
                allErrors,
                grammarNameChanges,
                testNameChanges,
                (errors, grammarName, testName) =>
                {
                    return errors.Select(e =>
                    {
                        switch (e.FileName)
                        {
                            case Pipeline.PegParser.SentinelFileName:
                                return new CompilerError(grammarName, e.Line, e.Column, e.ErrorNumber, e.ErrorText) { IsWarning = e.IsWarning };

                            case Pipeline.CsCompiler.SentinelFileName:
                                return new CompilerError(grammarName + ".g.cs", e.Line, e.Column, e.ErrorNumber, e.ErrorText) { IsWarning = e.IsWarning };

                            case Pipeline.TestParser.SentinelFileName:
                                return new CompilerError(testName, e.Line, e.Column, e.ErrorNumber, e.ErrorText) { IsWarning = e.IsWarning };

                            default:
                                return e;
                        }
                    }).ToList();
                });

            this.pipeline = new CompositeDisposable(
                testResults.BindTo(this, x => x.TestResults),
                compileErrors.BindTo(this, x => x.CompileErrors));

            this.Save = ReactiveCommand.CreateAsyncTask(grammarNameChanges.Select(n => n != "Untitled.peg"), async _ =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.grammarFileName));
                await FileUtilities.WriteAllTextAsync(this.grammarFileName, this.grammarText);
                this.GrammarChanged = false;
            });

            this.SaveAs = ReactiveCommand.CreateAsyncTask(async f =>
            {
                var fileName = (string)f;
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                await FileUtilities.WriteAllTextAsync(this.grammarFileName, this.grammarText);
                this.GrammarFileName = fileName;
                this.GrammarChanged = false;
                return true;
            });

            this.Load = ReactiveCommand.CreateAsyncTask(async f =>
            {
                var fileName = (string)f;
                this.GrammarText = await FileUtilities.ReadAllTextAsync(fileName);
                this.GrammarFileName = fileName;
                this.GrammarChanged = false;
                this.TestText = string.Empty;
                return true;
            });

            this.LoadTutorial = ReactiveCommand.CreateAsyncTask(async t =>
            {
                var tutorial = (Tutorial)t;
                this.GrammarText = File.Exists(tutorial.FileName) ? await FileUtilities.ReadAllTextAsync(tutorial.FileName) : tutorial.GrammarText;
                this.GrammarFileName = tutorial.FileName;
                this.GrammarChanged = false;
                this.TestText = tutorial.TestText;
                return true;
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
        public IReactiveCommand Load { get; }

        /// <summary>
        /// Gets the load tutorial command.
        /// </summary>
        public IReactiveCommand LoadTutorial { get; }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        public IReactiveCommand Save { get; }

        /// <summary>
        /// Gets the save-as command.
        /// </summary>
        public IReactiveCommand SaveAs { get; }

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
        /// Gets the list of tutorials.
        /// </summary>
        public IReadOnlyList<Tutorial> Tutorials { get; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose() => this.pipeline.Dispose();

        private class CompilerErrorListEqualityComparer : IEqualityComparer<IList<CompilerError>>
        {
            public bool Equals(IList<CompilerError> x, IList<CompilerError> y)
            {
                if (x != null && y != null && x.Count == 0 && y.Count == 0)
                {
                    return true;
                }

                return object.Equals(x, y);
            }

            public int GetHashCode(IList<CompilerError> obj)
            {
                return obj == null || obj.Count == 0 ? 0 : obj.GetHashCode();
            }
        }
    }
}
