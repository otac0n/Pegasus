// -----------------------------------------------------------------------
// <copyright file="IParseResult{T}.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Common
{
    /// <summary>
    /// Encapsulates the success or failure of a particular parsing operation along with the result of operation.
    /// </summary>
    /// <typeparam name="T">The type of the parsing operation's result.</typeparam>
    public interface IParseResult<out T>
    {
        /// <summary>
        /// Gets the ending cursor of the match.
        /// </summary>
        Cursor EndCursor { get; }

        /// <summary>
        /// Gets the starting cursor of the match.
        /// </summary>
        Cursor StartCursor { get; }

        /// <summary>
        /// Gets the resulting value of the parsing operation.
        /// </summary>
        T Value { get; }
    }
}
