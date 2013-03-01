// -----------------------------------------------------------------------
// <copyright file="SequenceExpression.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a sequence of expressions to match.
    /// </summary>
    public class SequenceExpression : Expression
    {
        private readonly IList<Expression> sequence;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceExpression"/> class.
        /// </summary>
        /// <param name="sequence">The sequence of expressions to match.</param>
        public SequenceExpression(IEnumerable<Expression> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            this.sequence = sequence.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the sequence of expressions to match.
        /// </summary>
        public IList<Expression> Sequence
        {
            get { return this.sequence; }
        }
    }
}
