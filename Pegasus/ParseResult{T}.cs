// -----------------------------------------------------------------------
// <copyright file="ParseResult{T}.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System;

    /// <summary>
    /// Encapsulates the success or failure of a particular parsing operation along with the result of operation.
    /// </summary>
    /// <typeparam name="T">The type of the parsing operation's result.</typeparam>
    public class ParseResult<T> : IEquatable<ParseResult<T>>
    {
        private readonly int length;
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult&lt;T&gt;"/> class.
        /// </summary>
        /// <remarks>
        /// A non-null parse result indicates success, whereas a null result indicates failure.
        /// </remarks>
        /// <param name="length">The lenght of the match.</param>
        /// <param name="value">The value of the match.</param>
        public ParseResult(int length, T value)
        {
            this.length = length;
            this.value = value;
        }

        /// <summary>
        /// Gets the number of bytes or characters consumed by the parsing operation.
        /// </summary>
        public int Length
        {
            get
            {
                return this.length;
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
        /// <param name="a">The first <see cref="ParseResult&lt;T&gt;"/> to compare, or null.</param>
        /// <param name="b">The second <see cref="ParseResult&lt;T&gt;"/> to compare, or null.</param>
        /// <returns>true if the value of a is the same as the value of b; otherwise, false.</returns>
        public static bool operator ==(ParseResult<T> a, ParseResult<T> b)
        {
            return object.Equals(a, b);
        }

        /// <summary>
        /// Determines whether two specified parse results have different values.
        /// </summary>
        /// <param name="a">The first <see cref="ParseResult&lt;T&gt;"/> to compare, or null.</param>
        /// <param name="b">The second <see cref="ParseResult&lt;T&gt;"/> to compare, or null.</param>
        /// <returns>true if the value of a is different from the value of b; otherwise, false.</returns>
        public static bool operator !=(ParseResult<T> a, ParseResult<T> b)
        {
            return !object.Equals(a, b);
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
        /// <param name="other">An <see cref="ParseResult&lt;T&gt;"/> to compare with this <see cref="ParseResult&lt;T&gt;"/>.</param>
        /// <returns>true if the parse results are considered equal; otherwise, false.</returns>
        public bool Equals(ParseResult<T> other)
        {
            return !object.ReferenceEquals(other, null) &&
                this.length == other.length &&
                object.Equals(this.value, other.value);
        }

        /// <summary>
        /// Serves as a hash function for this <see cref="ParseResult&lt;T&gt;"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="ParseResult&lt;T&gt;"/>.</returns>
        public override int GetHashCode()
        {
            int hash = 0x51ED270B;
            hash = (hash * -0x25555529) + this.length.GetHashCode();
            hash = (hash * -0x25555529) + (object.ReferenceEquals(this.value, null) ? 0 : this.value.GetHashCode());

            return hash;
        }
    }
}
