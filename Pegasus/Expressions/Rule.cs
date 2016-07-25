// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Represents a parse rule.
    /// </summary>
    [DebuggerDisplay("Rule {Identifier.Name}")]
    public class Rule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that represents the <see cref="Rule"/>.</param>
        /// <param name="expression">The expression that this <see cref="Rule"/> represents.</param>
        /// <param name="flags" >The flags to be set on this <see cref="Rule"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "The usage of 'flags' here is intentional and desired.")]
        public Rule(Identifier identifier, Expression expression, IEnumerable<Identifier> flags)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            this.Identifier = identifier;
            this.Expression = expression;
            this.Flags = (flags ?? Enumerable.Empty<Identifier>()).ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the expression that this <see cref="Rule"/> represents.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the flags that have been set on this <see cref="Rule"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "The usage of 'Flags' here is intentional and desired.")]
        public IList<Identifier> Flags { get; }

        /// <summary>
        /// Gets the name of this <see cref="Rule"/>.
        /// </summary>
        public Identifier Identifier { get; }
    }
}
