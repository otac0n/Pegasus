// -----------------------------------------------------------------------
// <copyright file="LeftRecursionDetector.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Provides left recursion detection services for Pegasus <see cref="Grammar"/>s.
    /// </summary>
    public static class LeftRecursionDetector
    {
        /// <summary>
        /// Detects which rules in a <see cref="Grammar"/> are left recursive.
        /// </summary>
        /// <param name="grammar">The <see cref="Grammar"/> to inspect.</param>
        /// <returns>A <see cref="HashSet{T}"/> containing the left-recursive rules.</returns>
        public static HashSet<Rule> Detect(Grammar grammar)
        {
            var leftRecursive = new HashSet<Rule>();
            var zeroWidth = ZeroWidthEvaluator.Evaluate(grammar);
            new LeftRecursionExpressionTreeWalker(grammar, zeroWidth, leftRecursive).WalkGrammar(grammar);
            return leftRecursive;
        }

        private class LeftRecursionExpressionTreeWalker : ExpressionTreeWalker
        {
            private readonly Dictionary<string, Rule> rules;
            private readonly Dictionary<Expression, bool> zeroWidth;
            private readonly HashSet<Rule> leftRecursive;

            private int index = 0;
            private Rule v;
            private Dictionary<Rule, RuleData> ruleData = new Dictionary<Rule, RuleData>();
            private Stack<Rule> ruleStack = new Stack<Rule>();

            public LeftRecursionExpressionTreeWalker(Grammar grammar, Dictionary<Expression, bool> zeroWidth, HashSet<Rule> leftRecursive)
            {
                this.rules = grammar.Rules.ToDictionary(r => r.Identifier.Name);
                this.zeroWidth = zeroWidth;
                this.leftRecursive = leftRecursive;
            }

            public override void WalkGrammar(Grammar grammar)
            {
                foreach (var rule in grammar.Rules)
                {
                    if (!this.ruleData.ContainsKey(rule))
                    {
                        this.WalkRule(rule);
                    }
                }
            }

            protected override void WalkRule(Rule rule)
            {
                this.v = rule;
                this.ruleStack.Push(rule);
                var data = this.ruleData[rule] = new RuleData
                {
                    Index = this.index++,
                    LowLink = int.MaxValue,
                    InStack = true,
                };

                base.WalkRule(rule);

                if (data.LowLink > data.Index)
                {
                    this.ruleStack.Pop();
                    data.InStack = false;
                }
                else if (data.LowLink == data.Index)
                {
                    while (true)
                    {
                        var w = this.ruleStack.Pop();
                        this.ruleData[w].InStack = false;
                        this.leftRecursive.Add(w);

                        if (w == rule)
                        {
                            break;
                        }
                    }
                }
            }

            protected override void WalkNameExpression(NameExpression nameExpression)
            {
                var rule = this.rules[nameExpression.Identifier.Name];
                RuleData data;
                if (!this.ruleData.TryGetValue(rule, out data))
                {
                    var v = this.v;
                    this.WalkRule(rule);
                    this.v = v;

                    this.ruleData[this.v].LowLink = Math.Min(this.ruleData[this.v].LowLink, this.ruleData[rule].LowLink);
                }
                else if (data.InStack)
                {
                    this.ruleData[this.v].LowLink = Math.Min(this.ruleData[this.v].LowLink, data.Index);
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

        private class RuleData
        {
            public int Index { get; set; }

            public int LowLink { get; set; }

            public bool InStack { get; set; }
        }
    }
}
