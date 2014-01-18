// -----------------------------------------------------------------------
// <copyright file="ReportZeroWidthRepetitionPass.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportZeroWidthRepetitionPass : CompilePass
    {
        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001", "PEG0002", "PEG0003" }; }
        }

        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0021", "PEG0022" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            var zeroWidth = ZeroWidthEvaluator.Evaluate(grammar);
            new ZeroWidthRepetitionTreeWalker(zeroWidth, result).WalkGrammar(grammar);
        }

        private class ZeroWidthRepetitionTreeWalker : ExpressionTreeWalker
        {
            private readonly CompileResult result;
            private readonly Dictionary<Expression, bool> zeroWidth;

            public ZeroWidthRepetitionTreeWalker(Dictionary<Expression, bool> zeroWidth, CompileResult result)
            {
                this.result = result;
                this.zeroWidth = zeroWidth;
            }

            protected override void WalkRepetitionExpression(RepetitionExpression repetitionExpression)
            {
                base.WalkRepetitionExpression(repetitionExpression);

                if (this.zeroWidth[repetitionExpression.Expression] && (repetitionExpression.Quantifier.Delimiter == null || this.zeroWidth[repetitionExpression.Quantifier.Delimiter]))
                {
                    if (repetitionExpression.Quantifier.Max == null)
                    {
                        var cursor = repetitionExpression.Quantifier.Start;
                        this.result.AddError(cursor, () => Resources.PEG0021_ZERO_WIDTH_REPETITION);
                    }
                    else if (repetitionExpression.Quantifier.Min != repetitionExpression.Quantifier.Max)
                    {
                        var cursor = repetitionExpression.Quantifier.Start;
                        this.result.AddWarning(cursor, () => Resources.PEG0022_ZERO_WIDTH_REPETITION);
                    }
                }
            }
        }
    }
}
