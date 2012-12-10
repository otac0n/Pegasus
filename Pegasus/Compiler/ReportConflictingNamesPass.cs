// -----------------------------------------------------------------------
// <copyright file="ReportConflictingNamesPass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportConflictingNamesPass : CompilePass
    {
        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0007" }; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            new ConflictingNamesTreeWalker(result).WalkGrammar(grammar);
        }

        private class ConflictingNamesTreeWalker : ExpressionTreeWalker
        {
            private readonly CompileResult result;
            private readonly HashSet<string> currentNames = new HashSet<string>();

            public ConflictingNamesTreeWalker(CompileResult result)
            {
                this.result = result;
            }

            protected override void WalkRule(Rule rule)
            {
                base.WalkRule(rule);
                this.currentNames.Clear();
            }

            protected override void WalkChoiceExpression(ChoiceExpression choiceExpression)
            {
                var namesCopy = new HashSet<string>(this.currentNames);
                foreach (var expression in choiceExpression.Choices)
                {
                    this.WalkExpression(expression);
                    this.currentNames.IntersectWith(namesCopy);
                }
            }

            protected override void WalkPrefixedExpression(PrefixedExpression prefixedExpression)
            {
                var success = this.currentNames.Add(prefixedExpression.Prefix.Name);
                this.currentNames.Add(prefixedExpression.Prefix.Name + "Start");
                this.currentNames.Add(prefixedExpression.Prefix.Name + "End");

                if (!success)
                {
                    var cursor = prefixedExpression.Prefix.Start;
                    this.result.AddError(cursor, () => Resources.PEG0007_PREFIX_ALREADY_DECLARED, prefixedExpression.Prefix.Name);
                }

                base.WalkPrefixedExpression(prefixedExpression);
            }
        }
    }
}
