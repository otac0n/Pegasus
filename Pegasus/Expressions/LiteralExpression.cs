// -----------------------------------------------------------------------
// <copyright file="LiteralExpression.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents a literal string.
    /// </summary>
    public class LiteralExpression : Expression
    {
        private readonly bool ignoreCase;
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralExpression"/> class.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <param name="ignoreCase">A value indicating whether or not the expression should ignore case differences when matching.</param>
        public LiteralExpression(string value, bool ignoreCase = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.value = value;
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
        /// Gets the value of this expression.
        /// </summary>
        public string Value
        {
            get { return this.value; }
        }
    }
}
