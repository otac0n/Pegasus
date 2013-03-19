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
    using Pegasus.Common;

    /// <summary>
    /// Represents a literal string.
    /// </summary>
    public class LiteralExpression : Expression
    {
        private readonly Cursor end;
        private readonly bool fromResource;
        private readonly bool ignoreCase;
        private readonly Cursor start;
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralExpression"/> class.
        /// </summary>
        /// <param name="start">The cursor just before the <see cref="LiteralExpression"/>.</param>
        /// <param name="end">The cursor just after the <see cref="LiteralExpression"/>.</param>
        /// <param name="value">The literal value.</param>
        /// <param name="ignoreCase">A value indicating whether or not the expression should ignore case differences when matching.</param>
        /// <param name="fromResource">a value indicating whether <paramref name="value"/> corresponds to a resource name or a literal value.</param>
        public LiteralExpression(Cursor start, Cursor end, string value, bool ignoreCase = false, bool fromResource = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.start = start;
            this.end = end;
            this.value = value;
            this.ignoreCase = ignoreCase;
            this.fromResource = fromResource;
        }

        /// <summary>
        /// Gets the cursor just after the <see cref="LiteralExpression"/>.
        /// </summary>
        public Cursor End
        {
            get { return this.end; }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Value"/> corresponds to a resource name or a literal value
        /// </summary>
        /// <value>
        /// true, if <see cref="Value"/> corresponds to a resource name that will be used as the literal value; false, otherwise.
        /// </value>
        public bool FromResource
        {
            get { return this.fromResource; }
        }

        /// <summary>
        /// Gets a value indicating whether the expression should ignore case differences when matching.
        /// </summary>
        public bool IgnoreCase
        {
            get { return this.ignoreCase; }
        }

        /// <summary>
        /// Gets the cursor just before the <see cref="LiteralExpression"/>.
        /// </summary>
        public Cursor Start
        {
            get { return this.start; }
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
