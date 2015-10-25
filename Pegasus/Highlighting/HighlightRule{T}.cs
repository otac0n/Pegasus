// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Highlighting
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a rule for highlighting.
    /// </summary>
    /// <typeparam name="T">The type of the value of the match.</typeparam>
    public class HighlightRule<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightRule{T}"/> class.
        /// </summary>
        /// <param name="pattern">The <see cref="Regex"/> pattern to use for matching.</param>
        /// <param name="value">The value of the match.</param>
        public HighlightRule(string pattern, T value)
        {
            this.Pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
            this.Value = value;
        }

        /// <summary>
        /// Gets the pattern to use for matching.
        /// </summary>
        public Regex Pattern { get; }

        /// <summary>
        /// Gets the value of the match.
        /// </summary>
        public T Value { get; }
    }
}
