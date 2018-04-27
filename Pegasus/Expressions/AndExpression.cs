// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents a positive look-ahead.
    /// </summary>
    public class AndExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndExpression"/> class.
        /// </summary>
        /// <param name="expression">An expression that must match at a location for this expression to match at that location.</param>
        public AndExpression(Expression expression)
        {
            this.Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        /// <summary>
        /// Gets the expression that must match at a location for this expression to match at that location.
        /// </summary>
        public Expression Expression { get; }
    }
}
