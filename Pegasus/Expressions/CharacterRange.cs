// -----------------------------------------------------------------------
// <copyright file="CharacterRange.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    /// <summary>
    /// Represents an inclusive range of characters.
    /// </summary>
    public class CharacterRange
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
    }
}
