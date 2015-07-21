// -----------------------------------------------------------------------
// <copyright file="SequenceExpression.cs" company="(none)">
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

            var flattened = new List<Expression>();
            foreach (var e in sequence)
            {
                SequenceExpression sequenceExpression;
                LiteralExpression literalExpression;
                if ((sequenceExpression = e as SequenceExpression) != null)
                {
                    var last = sequenceExpression.Sequence.LastOrDefault() as CodeExpression;
                    if (last == null || last.CodeType != CodeType.Result)
                    {
                        flattened.AddRange(sequenceExpression.Sequence);
                        continue;
                    }
                }
                else if ((literalExpression = e as LiteralExpression) != null)
                {
                    if (literalExpression.Value == string.Empty)
                    {
                        continue;
                    }
                }

                flattened.Add(e);
            }

            this.sequence = flattened.AsReadOnly();
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
