// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Common.Highlighting
{
    using System.Collections.Generic;

    /// <summary>
    /// A list of highlight rules.
    /// </summary>
    /// <typeparam name="T">The type of the value of the match of each of the rules.</typeparam>
    public class HighlightRuleCollection<T> : List<HighlightRule<T>>
    {
        /// <summary>
        /// Adds a rule with the specified pattern and value to the list.
        /// </summary>
        /// <param name="pattern">The pattern to use for matching.</param>
        /// <param name="value">The value of the match.</param>
        public void Add(string pattern, T value)
        {
            this.Add(new HighlightRule<T>(pattern, value));
        }
    }
}
