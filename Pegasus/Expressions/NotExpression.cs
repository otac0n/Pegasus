// -----------------------------------------------------------------------
// <copyright file="NotExpression.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents a negative lookahead.
    /// </summary>
    public class NotExpression : Expression
    {
        private readonly Expression expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotExpression"/> class.
        /// </summary>
        /// <param name="expression">An expression that must not match at a location for this expression to match at that location.</param>
        public NotExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.expression = expression;
        }

        /// <summary>
        /// Gets the expression that must not match at a location for this expression to match at that location.
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }
    }
}
