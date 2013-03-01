// -----------------------------------------------------------------------
// <copyright file="RepetitionExpression.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
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
        private readonly int? max;
        private readonly int min;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepetitionExpression"/> class.
        /// </summary>
        /// <param name="expression">The expression to be repeatedly matched.</param>
        /// <param name="min">The minimum number of times to match.</param>
        /// <param name="max">The maximum number of times to match, if limited; or null, otherwise.</param>
        public RepetitionExpression(Expression expression, int min, int? max)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.expression = expression;
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Gets the expression to be repeatedly matched.
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the maximum number of times to match, if limited; or null, if there is no limit.
        /// </summary>
        public int? Max
        {
            get { return this.max; }
        }

        /// <summary>
        /// Gets the minimum number of times to match.
        /// </summary>
        public int Min
        {
            get { return this.min; }
        }
    }
}
