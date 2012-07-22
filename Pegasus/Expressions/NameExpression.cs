// -----------------------------------------------------------------------
// <copyright file="NameExpression.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents a reference to another expression by name.
    /// </summary>
    public class NameExpression : Expression
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NameExpression"/> class.
        /// </summary>
        /// <param name="name">The name of the referenced expression.</param>
        public NameExpression(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;
        }

        /// <summary>
        /// Gets the name of the referenced expression.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
    }
}
