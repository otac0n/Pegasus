// -----------------------------------------------------------------------
// <copyright file="Rule.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
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
    /// Represents a parse rule.
    /// </summary>
    public class Rule
    {
        private readonly Expression expression;
        private readonly IList<Identifier> flags;
        private readonly Identifier identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that represents the <see cref="Rule"/>.</param>
        /// <param name="expression">The expression that this <see cref="Rule"/> represents.</param>
        /// <param name="flags" >The flags to be set on this <see cref="Rule"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "The usage of 'flags' here is intentional and desired.")]
        public Rule(Identifier identifier, Expression expression, IEnumerable<Identifier> flags)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.identifier = identifier;
            this.expression = expression;
            this.flags = flags.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the expression that this <see cref="Rule"/> represents.
        /// </summary>
        public Expression Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the flags that have been set on this <see cref="Rule"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "The usage of 'Flags' here is intentional and desired.")]
        public IList<Identifier> Flags
        {
            get { return this.flags; }
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
