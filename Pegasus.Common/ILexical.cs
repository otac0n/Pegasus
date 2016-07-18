// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Common
{
    /// <summary>
    /// Marks a class as being a lexical element.
    /// </summary>
    public interface ILexical
    {
        /// <summary>
        /// Gets or sets the ending cursor of this instance.
        /// </summary>
        Cursor EndCursor { get; set; }

        /// <summary>
        /// Gets or sets the starting cursor of this instance.
        /// </summary>
        Cursor StartCursor { get; set; }
    }
}
