// -----------------------------------------------------------------------
// <copyright file="ReportLeftRecursionPass.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportLeftRecursionPass : CompilePass
    {
        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0004" }; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001", "PEG0002", "PEG0003" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            var zeroWidth = ZeroWidthEvaluator.Evaluate(grammar);
            new LeftRecursionExpressionTreeWalker(grammar, zeroWidth, result).WalkGrammar(grammar);
        }

        private class LeftRecursionExpressionTreeWalker : ExpressionTreeWalker
        {
            private readonly Dictionary<string, Rule> rules;
            private readonly Dictionary<Expression, bool> zeroWidth;
            private readonly CompileResult result;

            private Stack<Rule> ruleStack = new Stack<Rule>();
            private HashSet<Rule> checkedRules = new HashSet<Rule>();

            public LeftRecursionExpressionTreeWalker(Grammar grammar, Dictionary<Expression, bool> zeroWidth, CompileResult result)
            {
                this.rules = grammar.Rules.ToDictionary(r => r.Identifier.Name);
                this.zeroWidth = zeroWidth;
                this.result = result;
            }

            protected override void WalkRule(Rule rule)
            {
                if (!this.checkedRules.Add(rule))
                {
                    return;
                }

                this.ruleStack.Push(rule);
                base.WalkRule(rule);
                this.ruleStack.Pop();
            }

            protected override void WalkNameExpression(NameExpression nameExpression)
            {
                var rule = this.rules[nameExpression.Identifier.Name];
                if (this.ruleStack.Contains(rule))
                {
                    this.ruleStack.Push(rule);
                    var names = string.Join(" -> ", this.ruleStack.Reverse().SkipWhile(r => r != rule).Select(r => r.Identifier.Name));
                    this.ruleStack.Pop();

                    var cursor = rule.Identifier.Start;
                    this.result.AddError(cursor, () => Resources.PEG0004_LEFT_RECURSION_DETECTED, names);
                }
                else
                {
                    this.WalkRule(rule);
                }
            }

            protected override void WalkSequenceExpression(SequenceExpression sequenceExpression)
            {
                foreach (var expression in sequenceExpression.Sequence)
                {
                    this.WalkExpression(expression);

                    if (!this.zeroWidth[expression])
                    {
                        break;
                    }
                }
            }
        }
    }
}
