// -----------------------------------------------------------------------
// <copyright file="ReportMissingRulesPass.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportMissingRulesPass : CompilePass
    {
        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001" }; }
        }

        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0003" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            new MissingRuleExpressionTreeWalker(grammar, result).WalkGrammar(grammar);
        }

        private class MissingRuleExpressionTreeWalker : ExpressionTreeWalker
        {
            private readonly HashSet<string> knownRules;
            private readonly CompileResult result;

            public MissingRuleExpressionTreeWalker(Grammar grammar, CompileResult result)
            {
                this.knownRules = new HashSet<string>(grammar.Rules.Select(r => r.Identifier.Name));
                this.result = result;
            }

            protected override void WalkNameExpression(NameExpression nameExpression)
            {
                var name = nameExpression.Identifier.Name;
                if (!this.knownRules.Contains(name))
                {
                    var cursor = nameExpression.Identifier.Start;
                    this.result.AddError(cursor, () => Resources.PEG0003_RULE_DOES_NOT_EXIST, name);
                }
            }
        }
    }
}
