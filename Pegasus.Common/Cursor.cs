// -----------------------------------------------------------------------
// <copyright file="Cursor.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a location within a parsing subject.
    /// </summary>
#if !PORTABLE

    [Serializable]
#endif
    public class Cursor : IEquatable<Cursor>
    {
        private readonly int column;
        private readonly string fileName;
        private readonly int line;
        private readonly int location;
        private readonly bool inTransition;
        private readonly string subject;
        private readonly IDictionary<string, object> state;
        private readonly bool mutable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cursor"/> class.
        /// </summary>
        /// <param name="subject">The parsing subject.</param>
        /// <param name="location">The location within the parsing subject.</param>
        /// <param name="fileName">The filename of the subject.</param>
        public Cursor(string subject, int location, string fileName = null)
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            if (location < 0 || location > subject.Length)
            {
                throw new ArgumentOutOfRangeException("location");
            }

            this.subject = subject;
            this.location = location;
            this.fileName = fileName;

            var line = 1;
            var column = 1;
            var inTransition = false;
            TrackLines(this.subject, 0, location, ref line, ref column, ref inTransition);

            this.line = line;
            this.column = column;
            this.inTransition = inTransition;
            this.state = new Dictionary<string, object>();
            this.mutable = false;
        }

        private Cursor(string subject, int location, string fileName, int line, int column, bool inTransition, IDictionary<string, object> state, bool mutable)
        {
            this.subject = subject;
            this.location = location;
            this.fileName = fileName;
            this.line = line;
            this.column = column;
            this.inTransition = inTransition;
            this.state = state;
            this.mutable = mutable;
        }

        /// <summary>
        /// Gets the column number represented by the location.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Gets the filename of the parsing subject.
        /// </summary>
        public string FileName
        {
            get { return this.fileName; }
        }

        /// <summary>
        /// Gets the line number of the cursor.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Gets the location within the parsing subject.
        /// </summary>
        public int Location
        {
            get { return this.location; }
        }

        /// <summary>
        /// Gets a hash code that varies with this cursor's state object.
        /// </summary>
        /// <remarks>This value, along with this cursor's location uniquely identify the parsing state.</remarks>
        public int StateKey
        {
            get { return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this.state); }
        }

        /// <summary>
        /// Gets the parsing subject.
        /// </summary>
        public string Subject
        {
            get { return this.subject; }
        }

        /// <summary>
        /// Gets the state value with the specified key.
        /// </summary>
        /// <param name="key">The key of the state value.</param>
        /// <returns>The state vale.</returns>
        public dynamic this[string key]
        {
            get
            {
                object value;
                this.state.TryGetValue(key, out value);
                return value;
            }

            set
            {
                if (!this.mutable)
                {
                    throw new InvalidOperationException();
                }

                this.state[key] = value;
            }
        }

        /// <summary>
        /// Determines whether two specified cursors represent the same location.
        /// </summary>
        /// <param name="left">The first <see cref="Cursor"/> to compare, or null.</param>
        /// <param name="right">The second <see cref="Cursor"/> to compare, or null.</param>
        /// <returns>true if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator ==(Cursor left, Cursor right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified cursors represent different locations.
        /// </summary>
        /// <param name="left">The first <see cref="Cursor"/> to compare, or null.</param>
        /// <param name="right">The second <see cref="Cursor"/> to compare, or null.</param>
        /// <returns>true if the value of <paramref name="left"/> is different from the value of <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator !=(Cursor left, Cursor right)
        {
            return !object.Equals(left, right);
        }

        /// <summary>
        /// Returns a new <see cref="Cursor"/> representing the location after consuming the given <see cref="ParseResult&lt;T&gt;"/>.
        /// </summary>
        /// <param name="count">The number of characters to advance.</param>
        /// <returns>A <see cref="Cursor"/> that represents the location after consuming the given <see cref="ParseResult&lt;T&gt;"/>.</returns>
        public Cursor Advance(int count)
        {
            if (this.mutable)
            {
                throw new InvalidOperationException();
            }

            var line = this.line;
            var column = this.column;
            var inTransition = this.inTransition;
            TrackLines(this.subject, this.location, count, ref line, ref column, ref inTransition);

            return new Cursor(this.subject, this.location + count, this.fileName, line, column, inTransition, this.state, this.mutable);
        }

        /// <summary>
        /// Returns a <see cref="Cursor"/> with the specified mutability.
        /// </summary>
        /// <param name="mutable">A value indicating whether or not the resulting <see cref="Cursor"/> should be mutable.</param>
        /// <returns>A <see cref="Cursor"/> with the specified mutability.</returns>
        public Cursor WithMutability(bool mutable)
        {
            return new Cursor(this.subject, this.location, this.fileName, this.line, this.column, this.inTransition, this.state.ToDictionary(i => i.Key, i => i.Value), mutable);
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
                this.subject == other.subject &&
                this.fileName == other.fileName;
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
            hash = (hash * -0x25555529) + (this.fileName == null ? 0 : this.fileName.GetHashCode());

            return hash;
        }

        private static void TrackLines(string subject, int start, int count, ref int line, ref int column, ref bool inTransition)
        {
            if (count == 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                var c = subject[start + i];
                if (c == '\r' || c == '\n')
                {
                    if (inTransition)
                    {
                        inTransition = false;
                        line++;
                        column = 1;
                    }
                    else if (subject.Length <= start + i + 1)
                    {
                        line++;
                        column = 1;
                    }
                    else
                    {
                        var peek = subject[start + i + 1];
                        if ((c == '\r' && peek == '\n') ||
                            (c == '\n' && peek == '\r'))
                        {
                            inTransition = true;
                            column++;
                        }
                        else
                        {
                            line++;
                            column = 1;
                        }
                    }
                }
                else if (c == '\u2028' || c == '\u2029')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
            }
        }
    }
}
