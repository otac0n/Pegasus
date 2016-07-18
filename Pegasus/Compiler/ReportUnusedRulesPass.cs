// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportUnusedRulesPass : CompilePass
    {
        public override IList<string> BlockedByErrors => new[] { "PEG0001", "PEG0002", "PEG0003" };

        public override IList<string> ErrorsProduced => new[] { "PEG0017" };

        public override void Run(Grammar grammar, CompileResult result) => new UnusedRulesExpressionTreeWalker(result).WalkGrammar(grammar);

        private class UnusedRulesExpressionTreeWalker : ExpressionTreeWalker
        {
            private CompileResult result;
            private Queue<string> rulesToVisit = new Queue<string>();
            private HashSet<string> usedRules = new HashSet<string>();

            public UnusedRulesExpressionTreeWalker(CompileResult result)
            {
                this.result = result;
            }

            public override void WalkGrammar(Grammar grammar)
            {
                var rules = grammar.Rules.ToDictionary(r => r.Identifier.Name, r => r);

                var startRule = grammar.Settings.Where(s => s.Key.Name == "start").Select(s => s.Value.ToString()).SingleOrDefault() ?? grammar.Rules[0].Identifier.Name;
                this.usedRules.Add(startRule);
                this.rulesToVisit.Enqueue(startRule);

                while (this.rulesToVisit.Count > 0)
                {
                    var ruleName = this.rulesToVisit.Dequeue();
                    this.WalkRule(rules[ruleName]);
                }

                var unusedRules = new HashSet<string>(grammar.Rules.Select(r => r.Identifier.Name));
                unusedRules.ExceptWith(this.usedRules);

                foreach (var ruleName in unusedRules)
                {
                    var rule = rules[ruleName];
                    this.result.AddCompilerError(rule.Identifier.Start, () => Resources.PEG0017_WARNING_UnusedRule, rule.Identifier.Name);
                }
            }

            protected override void WalkNameExpression(NameExpression nameExpression)
            {
                var name = nameExpression.Identifier.Name;
                if (this.usedRules.Add(name))
                {
                    this.rulesToVisit.Enqueue(name);
                }
            }
        }
    }
}
