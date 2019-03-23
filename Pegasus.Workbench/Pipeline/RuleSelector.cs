// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Workbench.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Pegasus.Compiler;
    using Pegasus.Expressions;
    using Pegasus.Workbench.Pipeline.Model;
    using ReactiveUI;

    /// <summary>
    /// Controls the interactions with the rule selector.
    /// </summary>
    public sealed class RuleSelector : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable disposable;
        private IList<ParserEntrypoint> entrypoints;
        private ParserEntrypoint selectedEntrypoint = null;

        public RuleSelector(IObservable<dynamic> parsers, IObservable<Grammar> grammars)
        {
            var entrypoints = parsers
                .Zip(grammars, (parser, grammar) => new { parser, grammar })
                .Select(p =>
                {
                    var list = new List<ParserEntrypoint>();

                    if (p.grammar != null)
                    {
                        var rules = PublicRuleFinder.Find(p.grammar);

                        if (rules.StartRule != null)
                        {
                            list.Add(new StartRuleEntrypoint(p.parser, rules.StartRule));
                        }

                        list.AddRange(rules.PublicRules.Select(rule => new PublicRuleEntrypoint(p.parser, rule)));
                        list.AddRange(rules.ExportedRules.Select(rule => new ExportedRuleEntrypoint(p.parser, rule)));
                    }

                    return list;
                });

            this.SelectedEntrypoints = this.WhenAny(x => x.SelectedEntrypoint, selectedEntrypoint => selectedEntrypoint.GetValue());

            var intentDetector = Observable.CombineLatest(
                entrypoints,
                this.SelectedEntrypoints.StartWith(default(ParserEntrypoint)),
                this.SelectedEntrypoints.Where(e => e != null).StartWith(default(ParserEntrypoint)),
                (list, current, intent) => new { list, current, intent });

            this.disposable = new CompositeDisposable(
                entrypoints.BindTo(this, x => x.Entrypoints),
                intentDetector.Subscribe(tuple =>
                {
                    if (tuple.current == null && tuple.list.Count > 0)
                    {
                        ParserEntrypoint intended = null;
                        if (tuple.intent != null)
                        {
                            intended = tuple.list.Where(e => ParserEntrypointEqualityComparer.Instance.Equals(e, tuple.intent)).FirstOrDefault();
                        }

                        intended = intended ?? tuple.list.FirstOrDefault();

                        if (intended != null)
                        {
                            this.SelectedEntrypoint = intended;
                        }
                    }
                }));
        }

        /// <summary>
        /// Gets the entrypoints.
        /// </summary>
        public IList<ParserEntrypoint> Entrypoints
        {
            get { return this.entrypoints; }
            private set { this.RaiseAndSetIfChanged(ref this.entrypoints, value); }
        }

        /// <summary>
        /// Gets or sets the selected rule.
        /// </summary>
        public ParserEntrypoint SelectedEntrypoint
        {
            get { return this.selectedEntrypoint; }
            set { this.RaiseAndSetIfChanged(ref this.selectedEntrypoint, value); }
        }

        /// <summary>
        /// Gets the selected entrypoints.
        /// </summary>
        public IObservable<ParserEntrypoint> SelectedEntrypoints { get; }

        /// <inheritdoc />
        public void Dispose() => this.disposable.Dispose();

        private class ParserEntrypointEqualityComparer : IEqualityComparer<ParserEntrypoint>
        {
            public static readonly ParserEntrypointEqualityComparer Instance = new ParserEntrypointEqualityComparer();

            private ParserEntrypointEqualityComparer()
            {
            }

            /// <inheritdoc />
            public bool Equals(ParserEntrypoint x, ParserEntrypoint y) => x?.Name == y?.Name;

            /// <inheritdoc />
            public int GetHashCode(ParserEntrypoint obj) => 0;
        }
    }
}
