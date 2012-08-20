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
        private readonly string displayName;
        private readonly Expression expression;
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="name">The name of the <see cref="Rule"/>.</param>
        /// <param name="displayName">The display name of the <see cref="Rule"/>.</param>
        /// <param name="expression">The expression that this <see cref="Rule"/> represents.</param>
        public Rule(string name, string displayName, Expression expression)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;

            this.displayName = displayName;

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.expression = expression;
        }

        /// <summary>
        /// Gets the display name of this <see cref="Rule"/>.
        /// </summary>
        public string DisplayName
        {
            get { return this.displayName ?? this.name; }
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
        public string Name
        {
            get { return this.name; }
        }
    }
}
