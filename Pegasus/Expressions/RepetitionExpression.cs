// -----------------------------------------------------------------------
// <copyright file="RepetitionExpression.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents the repetition of an expression.
    /// </summary>
    public class RepetitionExpression : Expression
    {
        private readonly Expression expression;
        private readonly Quantifier quantifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepetitionExpression"/> class.
        /// </summary>
        /// <param name="expression">The expression to be repeatedly matched.</param>
        /// <param name="quantifier">The quantifier that specifies how many times to match and the delimiter of the matches.</param>
        public RepetitionExpression(Expression expression, Quantifier quantifier)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (quantifier == null)
            {
                throw new ArgumentNullException("quantifier");
            }

            this.expression = expression;
            this.quantifier = quantifier;
        }

        /// <summary>
        /// Gets the expression to be repeatedly matched.
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the quantifier that specifies how many times to match and the delimiter of the matches.
        /// </summary>
        public Quantifier Quantifier
        {
            get { return this.quantifier; }
        }
    }
}
