// -----------------------------------------------------------------------
// <copyright file="IParseResult{T}.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Parser
{
    /// <summary>
    /// Encapsulates the success or failure of a particular parsing operation along with the result of operation.
    /// </summary>
    /// <typeparam name="T">The type of the parsing operation's result.</typeparam>
    public interface IParseResult<out T>
    {
        /// <summary>
        /// Gets the number of bytes or characters consumed by the parsing operation.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the resulting value of the parsing operation.
        /// </summary>
        T Value { get; }
    }
}
