// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;
    using System.Diagnostics;
    using Pegasus.Common;

    /// <summary>
    /// Represents a lexical identifier.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class Identifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier"/> class.
        /// </summary>
        /// <param name="name">The name of the <see cref="Identifier"/>.</param>
        /// <param name="start">The cursor just before the <see cref="Identifier"/>.</param>
        /// <param name="end">The cursor just after the <see cref="Identifier"/>.</param>
        public Identifier(string name, Cursor start, Cursor end)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            this.Name = name;
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Gets the cursor just after the <see cref="Identifier"/>.
        /// </summary>
        public Cursor End { get; }

        /// <summary>
        /// Gets the name of the <see cref="Identifier"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the cursor just before the <see cref="Identifier"/>.
        /// </summary>
        public Cursor Start { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}
