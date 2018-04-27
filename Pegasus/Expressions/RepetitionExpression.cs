// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents the repetition of an expression.
    /// </summary>
    public class RepetitionExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepetitionExpression"/> class.
        /// </summary>
        /// <param name="expression">The expression to be repeatedly matched.</param>
        /// <param name="quantifier">The quantifier that specifies how many times to match and the delimiter of the matches.</param>
        public RepetitionExpression(Expression expression, Quantifier quantifier)
        {
            this.Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            this.Quantifier = quantifier ?? throw new ArgumentNullException(nameof(quantifier));
        }

        /// <summary>
        /// Gets the expression to be repeatedly matched.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the quantifier that specifies how many times to match and the delimiter of the matches.
        /// </summary>
        public Quantifier Quantifier { get; }
    }
}
