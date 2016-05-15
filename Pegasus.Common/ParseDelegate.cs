// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Common
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Attempts a parse at the specified cursor.
    /// </summary>
    /// <typeparam name="T">The type of the parsing operation's result.</typeparam>
    /// <param name="cursor">The cursor that will be updated upon a successful parse.</param>
    /// <returns>An <see cref="IParseResult{T}"/>, if the parse was successful; <c>null</c> otherwise.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Since it would be a breaking change, this will not be renamed.")]
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Necessary for best performance.")]
    public delegate IParseResult<T> ParseDelegate<T>(ref Cursor cursor);
}
