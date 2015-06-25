// -----------------------------------------------------------------------
// <copyright file="ClassExpression.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a of a single character within certain character ranges.
    /// </summary>
    public class ClassExpression : Expression
    {
        private readonly bool ignoreCase;
        private readonly bool negated;
        private readonly IList<CharacterRange> ranges;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExpression"/> class.
        /// </summary>
        /// <param name="ranges">The ranges that match.</param>
        /// <param name="negated">A value indicating whether or not the expression is negated.</param>
        /// <param name="ignoreCase">A value indicating whether or not the expression should ignore case differences when matching.</param>
        public ClassExpression(IEnumerable<CharacterRange> ranges, bool negated = false, bool ignoreCase = false)
        {
            if (ranges == null)
            {
                throw new ArgumentNullException("ranges");
            }

            this.ranges = ranges.ToList().AsReadOnly();
            this.negated = negated;
            this.ignoreCase = ignoreCase;
        }

        /// <summary>
        /// Gets a value indicating whether the expression should ignore case differences when matching.
        /// </summary>
        public bool IgnoreCase
        {
            get { return this.ignoreCase; }
        }

        /// <summary>
        /// Gets a value indicating whether this expression is negated.
        /// </summary>
        public bool Negated
        {
            get { return this.negated; }
        }

        /// <summary>
        /// Gets the character ranges that match.
        /// </summary>
        public IList<CharacterRange> Ranges
        {
            get { return this.ranges; }
        }
    }
}
