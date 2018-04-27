// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

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
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceExpression"/> class.
        /// </summary>
        /// <param name="sequence">The sequence of expressions to match.</param>
        public SequenceExpression(IEnumerable<Expression> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            var flattened = new List<Expression>();
            foreach (var e in sequence)
            {
                LiteralExpression literalExpression;
                if (e is SequenceExpression sequenceExpression)
                {
                    var last = sequenceExpression.Sequence.LastOrDefault() as CodeExpression;
                    if (last == null || last.CodeType == CodeType.State || last.CodeType == CodeType.Error)
                    {
                        flattened.AddRange(sequenceExpression.Sequence);
                        continue;
                    }
                }
                else if ((literalExpression = e as LiteralExpression) != null)
                {
                    if (string.IsNullOrEmpty(literalExpression.Value))
                    {
                        continue;
                    }
                }

                flattened.Add(e);
            }

            this.Sequence = flattened.AsReadOnly();
        }

        /// <summary>
        /// Gets the sequence of expressions to match.
        /// </summary>
        public IList<Expression> Sequence { get; }
    }
}
