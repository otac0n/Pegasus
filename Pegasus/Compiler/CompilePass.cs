// -----------------------------------------------------------------------
// <copyright file="CompilePass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using Pegasus.Expressions;

    internal abstract class CompilePass
    {
        public abstract void Run(Grammar grammar, CompileResult result);

        protected static void WalkExpressions(Grammar grammar, Action<Expression> action)
        {
            foreach (var rule in grammar.Rules)
            {
                WalkExpression(rule.Expression, action);
            }
        }

        protected static void WalkExpression(Expression expression, Action<Expression> action)
        {
            action(expression);

            var andExpr = expression as AndExpression;
            var choiceExpr = expression as ChoiceExpression;
            var notExpr = expression as NotExpression;
            var prefixedExpr = expression as PrefixedExpression;
            var repetitionExpr = expression as RepetitionExpression;
            var sequenceExpr = expression as SequenceExpression;

            if (andExpr != null)
            {
                WalkExpression(andExpr.Expression, action);
            }
            else if (choiceExpr != null)
            {
                foreach (var choice in choiceExpr.Choices)
                {
                    WalkExpression(choice, action);
                }
            }
            else if (notExpr != null)
            {
                WalkExpression(notExpr.Expression, action);
            }
            else if (prefixedExpr != null)
            {
                WalkExpression(prefixedExpr.Expression, action);
            }
            else if (repetitionExpr != null)
            {
                WalkExpression(repetitionExpr.Expression, action);
            }
            else if (sequenceExpr != null)
            {
                foreach (var expr in sequenceExpr.Sequence)
                {
                    WalkExpression(expr, action);
                }
            }
        }
    }
}
