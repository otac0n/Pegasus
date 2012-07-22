// -----------------------------------------------------------------------
// <copyright file="ChoiceExpression.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
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
    /// Represents an ordered choice between a set of expressions.
    /// </summary>
    public class ChoiceExpression : Expression
    {
        private readonly IList<Expression> choices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceExpression"/> class.
        /// </summary>
        /// <param name="choices">The set of expressions to be used as choices.</param>
        public ChoiceExpression(IEnumerable<Expression> choices)
        {
            if (choices == null)
            {
                throw new ArgumentNullException("choices");
            }

            this.choices = choices.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the ordered set of choices that this expression can match.
        /// </summary>
        public IList<Expression> Choices
        {
            get { return this.choices; }
        }
    }
}
