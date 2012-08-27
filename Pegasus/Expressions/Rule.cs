// -----------------------------------------------------------------------
// <copyright file="Rule.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents a parse rule.
    /// </summary>
    public class Rule
    {
        private readonly Expression expression;
        private readonly Identifier identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that represents the <see cref="Rule"/>.</param>
        /// <param name="expression">The expression that this <see cref="Rule"/> represents.</param>
        public Rule(Identifier identifier, Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.identifier = identifier;
            this.expression = expression;
        }

        /// <summary>
        /// Gets the expression that this <see cref="Rule"/> represents.
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the name of this <see cref="Rule"/>.
        /// </summary>
        public Identifier Identifier
        {
            get { return this.identifier; }
        }
    }
}
