// -----------------------------------------------------------------------
// <copyright file="Identifier.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

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
        private readonly Cursor end;
        private readonly string name;
        private readonly Cursor start;

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
                throw new ArgumentNullException("name");
            }

            this.name = name;
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets the cursor just after the <see cref="Identifier"/>.
        /// </summary>
        public Cursor End
        {
            get { return this.end; }
        }

        /// <summary>
        /// Gets the name of the <see cref="Identifier"/>.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the cursor just before the <see cref="Identifier"/>.
        /// </summary>
        public Cursor Start
        {
            get { return this.start; }
        }
    }
}
