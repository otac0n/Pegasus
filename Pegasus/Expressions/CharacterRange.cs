// -----------------------------------------------------------------------
// <copyright file="CharacterRange.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents an inclusive range of characters.
    /// </summary>
    public class CharacterRange : IEquatable<CharacterRange>
    {
        private readonly char max;
        private readonly char min;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterRange"/> class.
        /// </summary>
        /// <param name="min">The minimum character value, inclusive.</param>
        /// <param name="max">The maximum character value, inclusive.</param>
        public CharacterRange(char min, char max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Gets the minimum character value, inclusive.
        /// </summary>
        public char Max
        {
            get { return this.max; }
        }

        /// <summary>
        /// Gets the maximum character value, inclusive.
        /// </summary>
        public char Min
        {
            get { return this.min; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="CharacterRange"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="CharacterRange"/>.</param>
        /// <returns>true if the specified <see cref="object"/> is equal to the current <see cref="CharacterRange"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as CharacterRange);
        }

        /// <summary>
        /// Determines whether the specified <see cref="CharacterRange"/> is equal to the current <see cref="CharacterRange"/>.
        /// </summary>
        /// <param name="other">The <see cref="CharacterRange"/> to compare with the current <see cref="CharacterRange"/>.</param>
        /// <returns>true if the specified <see cref="CharacterRange"/> is equal to the current <see cref="CharacterRange"/>; otherwise, false.</returns>
        public bool Equals(CharacterRange other)
        {
            return
                other != null &&
                other.min == this.min &&
                other.max == this.max;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            int hash = 0x51ED270B;
            hash = (hash * -0x25555529) + this.min.GetHashCode();
            hash = (hash * -0x25555529) + this.max.GetHashCode();
            return hash;
        }
    }
}
