// -----------------------------------------------------------------------
// <copyright file="Quantifier.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents the rules for repeating an expression.
    /// </summary>
    public class Quantifier
    {
        private readonly int? max;
        private readonly int min;
        private readonly Expression delimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quantifier"/> class.
        /// </summary>
        /// <param name="min">The minimum number of times to match.</param>
        /// <param name="max">The maximum number of times to match, if limited; or null, otherwise.</param>
        /// <param name="delimiter">The expression to use as a delimiter.</param>
        public Quantifier(int min, int? max = null, Expression delimiter = null)
        {
            this.min = min;
            this.max = max;
            this.delimiter = delimiter;
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

        /// <summary>
        /// Gets the expression to use as a delimiter.
        /// </summary>
        public Expression Delimiter
        {
            get { return this.delimiter; }
        }
    }
}
