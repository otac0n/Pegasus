// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

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
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExpression"/> class.
        /// </summary>
        /// <param name="ranges">The ranges that match.</param>
        /// <param name="negated">A value indicating whether or not the expression is negated.</param>
        /// <param name="ignoreCase">A value indicating whether or not the expression should ignore case differences when matching.</param>
        public ClassExpression(IEnumerable<CharacterRange> ranges, bool negated = false, bool? ignoreCase = null)
        {
            if (ranges == null)
            {
                throw new ArgumentNullException(nameof(ranges));
            }

            this.Ranges = ranges.ToList().AsReadOnly();
            this.Negated = negated;
            this.IgnoreCase = ignoreCase;
        }

        /// <summary>
        /// Gets a value indicating whether the expression should ignore case differences when matching.
        /// </summary>
        public bool? IgnoreCase { get; }

        /// <summary>
        /// Gets a value indicating whether this expression is negated.
        /// </summary>
        public bool Negated { get; }

        /// <summary>
        /// Gets the character ranges that match.
        /// </summary>
        public IList<CharacterRange> Ranges { get; }
    }
}
