﻿// -----------------------------------------------------------------------
// <copyright file="ReportInvalidQuantifiersPass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
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

    internal class ReportInvalidQuantifiersPass : CompilePass
    {
        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0015" }; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            new InvalidQuantifierTreeWalker(result).WalkGrammar(grammar);
        }

        private class InvalidQuantifierTreeWalker : ExpressionTreeWalker
        {
            private readonly CompileResult result;

            public InvalidQuantifierTreeWalker(CompileResult result)
            {
                this.result = result;
            }

            protected override void WalkRepetitionExpression(RepetitionExpression repetitionExpression)
            {
                if (repetitionExpression.Quantifier.Max == 0 ||
                    repetitionExpression.Quantifier.Max < repetitionExpression.Quantifier.Min)
                {
                    this.result.AddWarning(repetitionExpression.Quantifier.Start, () => Resources.PEG0015_QUANTIFIER_INVALID);
                }

                base.WalkRepetitionExpression(repetitionExpression);
            }
        }
    }
}