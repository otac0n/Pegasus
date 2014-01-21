// -----------------------------------------------------------------------
// <copyright file="LexicalElement.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Common
{
    using System.Diagnostics;

    /// <summary>
    /// A basic lexical element class that marks a region of text with a given name.
    /// </summary>
    [DebuggerDisplay("{Name}@{StartCursor.Location}:{EndCursor.Location}")]
    public class LexicalElement : ILexical
    {
        /// <summary>
        /// Gets or sets the ending cursor of this instance.
        /// </summary>
        public Cursor EndCursor { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the region of text.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the starting cursor of this instance.
        /// </summary>
        public Cursor StartCursor { get; set; }
    }
}
