// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportZeroWidthRepetitionPass : CompilePass
    {
        public override IList<string> BlockedByErrors => new[] { "PEG0001", "PEG0002", "PEG0003" };

        public override IList<string> ErrorsProduced => new[] { "PEG0021", "PEG0022" };

        public override void Run(Grammar grammar, CompileResult result)
        {
            var containsAssertions = ContainsAssertionsEvaluator.Evaluate(grammar);
            var zeroWidth = ZeroWidthEvaluator.Evaluate(grammar);
            new ZeroWidthRepetitionTreeWalker(containsAssertions, zeroWidth, result).WalkGrammar(grammar);
        }

        private class ZeroWidthRepetitionTreeWalker : ExpressionTreeWalker
        {
            private readonly Dictionary<Expression, bool> containsAssertions;
            private readonly CompileResult result;
            private readonly Dictionary<Expression, bool> zeroWidth;

            public ZeroWidthRepetitionTreeWalker(Dictionary<Expression, bool> containsAssertions, Dictionary<Expression, bool> zeroWidth, CompileResult result)
            {
                this.containsAssertions = containsAssertions;
                this.result = result;
                this.zeroWidth = zeroWidth;
            }

            protected override void WalkRepetitionExpression(RepetitionExpression repetitionExpression)
            {
                base.WalkRepetitionExpression(repetitionExpression);

                if (this.zeroWidth[repetitionExpression.Expression] && (repetitionExpression.Quantifier.Delimiter == null || this.zeroWidth[repetitionExpression.Quantifier.Delimiter]))
                {
                    var containsAssertions = this.containsAssertions[repetitionExpression.Expression] || (repetitionExpression.Quantifier.Delimiter != null && this.containsAssertions[repetitionExpression.Quantifier.Delimiter]);
                    var cursor = repetitionExpression.Quantifier.Start;

                    if (repetitionExpression.Quantifier.Max == null)
                    {
                        if (containsAssertions)
                        {
                            this.result.AddCompilerError(cursor, () => Resources.PEG0021_WARNING_ZeroWidthRepetition_Possible);
                        }
                        else
                        {
                            this.result.AddCompilerError(cursor, () => Resources.PEG0021_ERROR_ZeroWidthRepetition_Certain);
                        }
                    }
                    else if (repetitionExpression.Quantifier.Min != repetitionExpression.Quantifier.Max)
                    {
                        if (containsAssertions)
                        {
                            this.result.AddCompilerError(cursor, () => Resources.PEG0022_WARNING_ZeroWidthRepetition_Possible);
                        }
                        else
                        {
                            this.result.AddCompilerError(cursor, () => Resources.PEG0022_WARNING_ZeroWidthRepetition_Certain);
                        }
                    }
                }
            }
        }
    }
}
