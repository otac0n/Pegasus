// -----------------------------------------------------------------------
// <copyright file="HighlightRule{T}.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a rule for highlighting.
    /// </summary>
    /// <typeparam name="T">The type of the value of the match.</typeparam>
    public class HighlightRule<T>
    {
        private Regex pattern;
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightRule&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="pattern">The <see cref="Regex"/> pattern to use for matching.</param>
        /// <param name="value">The value of the match.</param>
        public HighlightRule(string pattern, T value)
        {
            this.pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
            this.value = value;
        }

        /// <summary>
        /// Gets the pattern to use for matching.
        /// </summary>
        public Regex Pattern
        {
            get { return this.pattern; }
        }

        /// <summary>
        /// Gets the value of the match.
        /// </summary>
        public T Value
        {
            get { return this.value; }
        }
    }
}
