// -----------------------------------------------------------------------
// <copyright file="TypedExpression.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents an expression with a specific type.
    /// </summary>
    public class TypedExpression : Expression
    {
        private readonly string type;
        private readonly Expression expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedExpression"/> class.
        /// </summary>
        /// <param name="type">The specific type of the wrapped expression.</param>
        /// <param name="expression">The wrapped expression.</param>
        public TypedExpression(string type, Expression expression)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException("type");
            }

            this.type = type;

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.expression = expression;
        }

        /// <summary>
        /// Gets the specific type of the wrapped expression.
        /// </summary>
        public string Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the wrapped expression.
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }
    }
}
