// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Provides left adjacency detection services for Pegasus <see cref="Grammar">Grammars</see>.
    /// </summary>
    public static class LeftAdjacencyDetector
    {
        /// <summary>
        /// Detects which expressions in a <see cref="Grammar"/> are left-adjacent.
        /// </summary>
        /// <param name="grammar">The <see cref="Grammar"/> to inspect.</param>
        /// <returns>A <see cref="ILookup{Rule, Expression}"/> containing the left-adjacent rules.</returns>
        public static ILookup<Rule, Expression> Detect(Grammar grammar)
        {
            var leftAdjacent = new Dictionary<Rule, List<Expression>>();
            var zeroWidth = ZeroWidthEvaluator.Evaluate(grammar);
            new LeftRecursionExpressionTreeWalker(zeroWidth, leftAdjacent).WalkGrammar(grammar);
            return leftAdjacent.SelectMany(i => i.Value, (i, v) => new { i.Key, Value = v }).ToLookup(i => i.Key, i => i.Value);
        }

        private class LeftRecursionExpressionTreeWalker : ExpressionTreeWalker
        {
            private readonly Dictionary<Rule, List<Expression>> leftAdjacent;
            private Rule currentRule;
            private Dictionary<Expression, bool> zeroWidth;

            public LeftRecursionExpressionTreeWalker(Dictionary<Expression, bool> zeroWidth, Dictionary<Rule, List<Expression>> leftAdjacent)
            {
                this.zeroWidth = zeroWidth;
                this.leftAdjacent = leftAdjacent;
            }

            public override void WalkExpression(Expression expression)
            {
                this.leftAdjacent[this.currentRule].Add(expression);
                base.WalkExpression(expression);
            }

            protected override void WalkRule(Rule rule)
            {
                this.currentRule = rule;
                this.leftAdjacent[this.currentRule] = new List<Expression>();
                base.WalkRule(rule);
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
