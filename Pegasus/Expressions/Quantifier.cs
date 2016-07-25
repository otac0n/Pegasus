// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;
    using Pegasus.Common;

    /// <summary>
    /// Represents the rules for repeating an expression.
    /// </summary>
    public class Quantifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Quantifier"/> class.
        /// </summary>
        /// <param name="start">The cursor just before the <see cref="Quantifier"/>.</param>
        /// <param name="end">The cursor just after the <see cref="Quantifier"/>.</param>
        /// <param name="min">The minimum number of times to match.</param>
        /// <param name="max">The maximum number of times to match, if limited; or null, otherwise.</param>
        /// <param name="delimiter">The expression to use as a delimiter.</param>
        public Quantifier(Cursor start, Cursor end, int min, int? max = null, Expression delimiter = null)
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            this.Start = start;
            this.End = end;
            this.Min = min;
            this.Max = max;

            if (delimiter != null)
            {
                SequenceExpression sequenceExpression;
                if ((sequenceExpression = delimiter as SequenceExpression) == null || sequenceExpression.Sequence.Count != 0)
                {
                    this.Delimiter = delimiter;
                }
            }
        }

        /// <summary>
        /// Gets the expression to use as a delimiter.
        /// </summary>
        public Expression Delimiter { get; }

        /// <summary>
        /// Gets the cursor just after the <see cref="Quantifier"/>.
        /// </summary>
        public Cursor End { get; }

        /// <summary>
        /// Gets the maximum number of times to match, if limited; or null, if there is no limit.
        /// </summary>
        public int? Max { get; }

        /// <summary>
        /// Gets the minimum number of times to match.
        /// </summary>
        public int Min { get; }

        /// <summary>
        /// Gets the cursor just before the <see cref="Quantifier"/>.
        /// </summary>
        public Cursor Start { get; }
    }
}
