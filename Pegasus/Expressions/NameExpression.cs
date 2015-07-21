// -----------------------------------------------------------------------
// <copyright file="NameExpression.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    /// <summary>
    /// Represents a reference to another expression by name.
    /// </summary>
    public class NameExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameExpression"/> class.
        /// </summary>
        /// <param name="identifier">The name of the referenced expression.</param>
        public NameExpression(Identifier identifier)
        {
            this.Identifier = identifier;
        }

        /// <summary>
        /// Gets the name of the referenced expression.
        /// </summary>
        public Identifier Identifier { get; }
    }
}
