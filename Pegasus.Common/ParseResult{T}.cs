// -----------------------------------------------------------------------
// <copyright file="ParseResult{T}.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Common
{
    using System;

    /// <summary>
    /// Encapsulates the success or failure of a particular parsing operation along with the result of operation.
    /// </summary>
    /// <typeparam name="T">The type of the parsing operation's result.</typeparam>
    public class ParseResult<T> : IParseResult<T>, IEquatable<ParseResult<T>>
    {
        private readonly Cursor endCursor;
        private readonly Cursor startCursor;
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult&lt;T&gt;"/> class.
        /// </summary>
        /// <remarks>
        /// A non-null parse result indicates success, whereas a null result indicates failure.
        /// </remarks>
        /// <param name="startCursor">The starting cursor of the match.</param>
        /// <param name="endCursor">The ending cursor of the match.</param>
        /// <param name="value">The value of the match.</param>
        public ParseResult(Cursor startCursor, Cursor endCursor, T value)
        {
            this.startCursor = startCursor;
            this.endCursor = endCursor;
            this.value = value;
        }

        /// <summary>
        /// Gets the ending cursor of the match.
        /// </summary>
        public Cursor EndCursor
        {
            get
            {
                return this.endCursor;
            }
        }

        /// <summary>
        /// Gets the starting cursor of the match.
        /// </summary>
        public Cursor StartCursor
        {
            get
            {
                return this.startCursor;
            }
        }

        /// <summary>
        /// Gets the resulting value of the parsing operation.
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Determines whether two specified parse results have the same value.
        /// </summary>
        /// <param name="left">The first <see cref="ParseResult&lt;T&gt;"/> to compare, or null.</param>
        /// <param name="right">The second <see cref="ParseResult&lt;T&gt;"/> to compare, or null.</param>
        /// <returns>true if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator ==(ParseResult<T> left, ParseResult<T> right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified parse results have different values.
        /// </summary>
        /// <param name="left">The first <see cref="ParseResult&lt;T&gt;"/> to compare, or null.</param>
        /// <param name="right">The second <see cref="ParseResult&lt;T&gt;"/> to compare, or null.</param>
        /// <returns>true if the value of <paramref name="left"/> is different from the value of <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator !=(ParseResult<T> left, ParseResult<T> right)
        {
            return !object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="ParseResult&lt;T&gt;"/>.
        /// </summary>
        /// <param name="obj">An object to compare with this <see cref="ParseResult&lt;T&gt;"/>.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ParseResult<T>);
        }

        /// <summary>
        /// Determines whether the specified <see cref="ParseResult&lt;T&gt;"/> is equal to the current <see cref="ParseResult&lt;T&gt;"/>.
        /// </summary>
        /// <param name="other">A <see cref="ParseResult&lt;T&gt;"/> to compare with this <see cref="ParseResult&lt;T&gt;"/>.</param>
        /// <returns>true if the parse results are considered equal; otherwise, false.</returns>
        public bool Equals(ParseResult<T> other)
        {
            return !object.ReferenceEquals(other, null) &&
                this.startCursor == other.startCursor &&
                this.endCursor == other.endCursor &&
                object.Equals(this.value, other.value);
        }

        /// <summary>
        /// Serves as a hash function for this <see cref="ParseResult&lt;T&gt;"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="ParseResult&lt;T&gt;"/>.</returns>
        public override int GetHashCode()
        {
            int hash = 0x51ED270B;
            hash = (hash * -0x25555529) + this.startCursor.GetHashCode();
            hash = (hash * -0x25555529) + this.endCursor.GetHashCode();
            hash = (hash * -0x25555529) + (object.ReferenceEquals(this.value, null) ? 0 : this.value.GetHashCode());

            return hash;
        }
    }
}
