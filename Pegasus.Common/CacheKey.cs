// -----------------------------------------------------------------------
// <copyright file="CacheKey.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Common
{
    using System.Diagnostics;

    /// <summary>
    /// A high-performance cache key for rule memoization.
    /// </summary>
    [DebuggerDisplay("{ruleName}:{location}:{stateKey}")]
    public class CacheKey
    {
        private readonly int hash;
        private readonly int location;
        private readonly string ruleName;
        private readonly int stateKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKey"/> class.
        /// </summary>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="stateKey">The state key of the current cursor.</param>
        /// <param name="location">The location of the current cursor.</param>
        public CacheKey(string ruleName, int stateKey, int location)
        {
            this.ruleName = ruleName;
            this.stateKey = stateKey;
            this.location = location;

            unchecked
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ (this.ruleName == null ? 0 : this.ruleName.GetHashCode());
                hash = hash * 16777619 ^ this.stateKey;
                hash = hash * 16777619 ^ this.location;
                this.hash = hash;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="CacheKey"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as CacheKey;
            if (!object.ReferenceEquals(other, null))
            {
                return
                    this.location == other.location &&
                    this.stateKey == other.stateKey &&
                    this.ruleName == other.ruleName;
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="CacheKey"/>.</returns>
        public override int GetHashCode()
        {
            return this.hash;
        }
    }
}
