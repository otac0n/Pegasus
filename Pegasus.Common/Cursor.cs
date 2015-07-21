// -----------------------------------------------------------------------
// <copyright file="Cursor.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Represents a location within a parsing subject.
    /// </summary>
#if !PORTABLE

    [Serializable]
#endif
    public class Cursor : IEquatable<Cursor>
    {
        private static int previousStateKey = -1;

        private readonly bool inTransition;
        private readonly bool mutable;
        private readonly IDictionary<string, object> state;
        private int stateKey;

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
                throw new ArgumentNullException(nameof(subject));
            }

            if (location < 0 || location > subject.Length)
            {
                throw new ArgumentOutOfRangeException("location");
            }

            this.Subject = subject;
            this.Location = location;
            this.FileName = fileName;

            var line = 1;
            var column = 1;
            var inTransition = false;
            TrackLines(this.Subject, 0, location, ref line, ref column, ref inTransition);

            this.Line = line;
            this.Column = column;
            this.inTransition = inTransition;
            this.state = new Dictionary<string, object>();
            this.stateKey = GetNextStateKey();
            this.mutable = false;
        }

        private Cursor(string subject, int location, string fileName, int line, int column, bool inTransition, IDictionary<string, object> state, int stateKey, bool mutable)
        {
            this.Subject = subject;
            this.Location = location;
            this.FileName = fileName;
            this.Line = line;
            this.Column = column;
            this.inTransition = inTransition;
            this.state = state;
            this.stateKey = stateKey;
            this.mutable = mutable;
        }

        /// <summary>
        /// Gets the column number represented by the location.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the filename of the parsing subject.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the line number of the cursor.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the location within the parsing subject.
        /// </summary>
        public int Location { get; }

        /// <summary>
        /// Gets a hash code that varies with this cursor's state object.
        /// </summary>
        /// <remarks>This value, along with this cursor's location uniquely identify the parsing state.</remarks>
        public int StateKey => this.stateKey;

        /// <summary>
        /// Gets the parsing subject.
        /// </summary>
        public string Subject { get; }

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

                this.stateKey = GetNextStateKey();
                this.state[key] = value;
            }
        }

        /// <summary>
        /// Determines whether two specified cursors represent different locations.
        /// </summary>
        /// <param name="left">The first <see cref="Cursor"/> to compare, or null.</param>
        /// <param name="right">The second <see cref="Cursor"/> to compare, or null.</param>
        /// <returns>true if the value of <paramref name="left"/> is different from the value of <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator !=(Cursor left, Cursor right) => !object.Equals(left, right);

        /// <summary>
        /// Determines whether two specified cursors represent the same location.
        /// </summary>
        /// <param name="left">The first <see cref="Cursor"/> to compare, or null.</param>
        /// <param name="right">The second <see cref="Cursor"/> to compare, or null.</param>
        /// <returns>true if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator ==(Cursor left, Cursor right) => object.Equals(left, right);

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

            var line = this.Line;
            var column = this.Column;
            var inTransition = this.inTransition;
            TrackLines(this.Subject, this.Location, count, ref line, ref column, ref inTransition);

            return new Cursor(this.Subject, this.Location + count, this.FileName, line, column, inTransition, this.state, this.stateKey, this.mutable);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Cursor"/>.
        /// </summary>
        /// <param name="obj">An object to compare with this <see cref="Cursor"/>.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public override bool Equals(object obj) => this.Equals(obj as Cursor);

        /// <summary>
        /// Determines whether the specified <see cref="Cursor"/> is equal to the current <see cref="Cursor"/>.
        /// </summary>
        /// <param name="other">A <see cref="Cursor"/> to compare with this <see cref="Cursor"/>.</param>
        /// <returns>true if the cursors represent the same location at the same state; otherwise, false.</returns>
        public bool Equals(Cursor other) =>
            !object.ReferenceEquals(other, null) &&
            this.Location == other.Location &&
            this.Subject == other.Subject &&
            this.FileName == other.FileName &&
            this.stateKey == other.stateKey;

        /// <summary>
        /// Serves as a hash function for this <see cref="Cursor"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Cursor"/>.</returns>
        public override int GetHashCode()
        {
            int hash = 0x51ED270B;
            hash = (hash * -0x25555529) + this.Subject.GetHashCode();
            hash = (hash * -0x25555529) + this.Location.GetHashCode();
            hash = (hash * -0x25555529) + (this.FileName == null ? 0 : this.FileName.GetHashCode());
            hash = (hash * -0x25555529) + this.stateKey;

            return hash;
        }

        /// <summary>
        /// Creates an identical cursor with a unique <see cref="StateKey"/>.
        /// </summary>
        /// <returns>A unique cursor.</returns>
        public Cursor Touch()
        {
            return new Cursor(this.Subject, this.Location, this.FileName, this.Line, this.Column, this.inTransition, this.mutable ? new Dictionary<string, object>(this.state) : this.state, GetNextStateKey(), this.mutable);
        }

        /// <summary>
        /// Returns a <see cref="Cursor"/> with the specified mutability.
        /// </summary>
        /// <param name="mutable">A value indicating whether or not the resulting <see cref="Cursor"/> should be mutable.</param>
        /// <returns>A <see cref="Cursor"/> with the specified mutability.</returns>
        public Cursor WithMutability(bool mutable)
        {
            return new Cursor(this.Subject, this.Location, this.FileName, this.Line, this.Column, this.inTransition, new Dictionary<string, object>(this.state), this.stateKey, mutable);
        }

        private static int GetNextStateKey() => Interlocked.Increment(ref previousStateKey);

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
