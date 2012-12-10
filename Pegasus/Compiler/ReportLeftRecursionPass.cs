// -----------------------------------------------------------------------
// <copyright file="ReportLeftRecursionPass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
            new LeftRecursionExpressionTreeWalker(grammar, result).WalkGrammar(grammar);
        }

        private class LeftRecursionExpressionTreeWalker : ExpressionTreeWalker
        {
            private readonly Dictionary<string, Rule> rules;
            private readonly CompileResult result;

            private Stack<Rule> ruleStack = new Stack<Rule>();
            private HashSet<Rule> checkedRules = new HashSet<Rule>();

            public LeftRecursionExpressionTreeWalker(Grammar grammar, CompileResult result)
            {
                this.rules = grammar.Rules.ToDictionary(r => r.Identifier.Name);
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

                    if (!this.IsExpressionZeroWidth(expression))
                    {
                        break;
                    }
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Empty strings and null strings are logically distinct in this method.")]
            private bool IsExpressionZeroWidth(Expression expression)
            {
                ChoiceExpression choiceExpression;
                LiteralExpression literalExpression;
                NameExpression nameExpression;
                PrefixedExpression prefixedExpression;
                RepetitionExpression repetitionExpression;
                SequenceExpression sequenceExpression;

                if (expression is AndCodeExpression || expression is AndExpression)
                {
                    return true;
                }
                else if ((choiceExpression = expression as ChoiceExpression) != null)
                {
                    return choiceExpression.Choices.Any(c => this.IsExpressionZeroWidth(c));
                }
                else if (expression is ClassExpression)
                {
                    return false;
                }
                else if (expression is CodeExpression)
                {
                    return true;
                }
                else if ((literalExpression = expression as LiteralExpression) != null)
                {
                    return literalExpression.Value == string.Empty;
                }
                else if ((nameExpression = expression as NameExpression) != null)
                {
                    return false; // this.IsExpressionZeroWidth(this.rules[nameExpression.Name]);
                }
                else if (expression is NotCodeExpression || expression is NotExpression)
                {
                    return true;
                }
                else if ((prefixedExpression = expression as PrefixedExpression) != null)
                {
                    return this.IsExpressionZeroWidth(prefixedExpression.Expression);
                }
                else if ((repetitionExpression = expression as RepetitionExpression) != null)
                {
                    return repetitionExpression.Min == 0 || this.IsExpressionZeroWidth(repetitionExpression.Expression);
                }
                else if ((sequenceExpression = expression as SequenceExpression) != null)
                {
                    return sequenceExpression.Sequence.All(e => this.IsExpressionZeroWidth(e));
                }
                else if (expression is WildcardExpression)
                {
                    return false;
                }
                else
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Unknown expression type '{0}'.", expression.GetType()), "expression");
                }
            }
        }
    }
}
