// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents a negative look-ahead.
    /// </summary>
    public class NotExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotExpression"/> class.
        /// </summary>
        /// <param name="expression">An expression that must not match at a location for this expression to match at that location.</param>
        public NotExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            this.Expression = expression;
        }

        /// <summary>
        /// Gets the expression that must not match at a location for this expression to match at that location.
        /// </summary>
        public Expression Expression { get; }
    }
}
