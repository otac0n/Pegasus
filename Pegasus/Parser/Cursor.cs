// -----------------------------------------------------------------------
// <copyright file="Cursor.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Parser
{
    using System;

    /// <summary>
    /// Represents a location within a parsing subject.
    /// </summary>
    public class Cursor : IEquatable<Cursor>
    {
        private readonly int location;
        private readonly string subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cursor"/> class.
        /// </summary>
        /// <param name="subject">The parsing subject.</param>
        /// <param name="location">The location within the parsing subject.</param>
        public Cursor(string subject, int location)
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            if (location < 0 || location > subject.Length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            this.subject = subject;
            this.location = location;
        }

        /// <summary>
        /// Gets the location within the parsing subject.
        /// </summary>
        public int Location
        {
            get { return this.location; }
        }

        /// <summary>
        /// Gets the parsing subject.
        /// </summary>
        public string Subject
        {
            get { return this.subject; }
        }

        /// <summary>
        /// Returns a new <see cref="Cursor"/> representing the location after consuming the given <see cref="ParseResult&lt;T&gt;"/>.
        /// </summary>
        /// <typeparam name="T">The parse result's value type.</typeparam>
        /// <param name="parseResult">The parse result.</param>
        /// <returns>A <see cref="Cursor"/> that represents the location after consuming the given <see cref="ParseResult&lt;T&gt;"/>.</returns>
        public Cursor Advance<T>(ParseResult<T> parseResult)
        {
            return new Cursor(this.subject, this.location + parseResult.Length);
        }

        /// <summary>
        /// Determines whether two specified cursors represent the same location.
        /// </summary>
        /// <param name="a">The first <see cref="Cursor"/> to compare, or null.</param>
        /// <param name="b">The second <see cref="Cursor"/> to compare, or null.</param>
        /// <returns>true if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>; otherwise, false.</returns>
        public static bool operator ==(Cursor a, Cursor b)
        {
            return object.Equals(a, b);
        }

        /// <summary>
        /// Determines whether two specified cursors represent different locations.
        /// </summary>
        /// <param name="a">The first <see cref="Cursor"/> to compare, or null.</param>
        /// <param name="b">The second <see cref="Cursor"/> to compare, or null.</param>
        /// <returns>true if the value of <paramref name="a"/> is different from the value of <paramref name="b"/>; otherwise, false.</returns>
        public static bool operator !=(Cursor a, Cursor b)
        {
            return !object.Equals(a, b);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Cursor"/>.
        /// </summary>
        /// <param name="obj">An object to compare with this <see cref="Cursor"/>.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Cursor);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Cursor"/> is equal to the current <see cref="Cursor"/>.
        /// </summary>
        /// <param name="other">A <see cref="Cursor"/> to compare with this <see cref="Cursor"/>.</param>
        /// <returns>true if the cursors represent the same location; otherwise, false.</returns>
        public bool Equals(Cursor other)
        {
            return !object.ReferenceEquals(other, null) &&
                this.location == other.location &&
                this.subject == other.subject;
        }

        /// <summary>
        /// Serves as a hash function for this <see cref="Cursor"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Cursor"/>.</returns>
        public override int GetHashCode()
        {
            int hash = 0x51ED270B;
            hash = (hash * -0x25555529) + this.subject.GetHashCode();
            hash = (hash * -0x25555529) + this.location.GetHashCode();

            return hash;
        }
    }
}
