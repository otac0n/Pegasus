// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Common;

    /// <summary>
    /// Represents a full set of grammar rules.
    /// </summary>
    public class Grammar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Grammar"/> class.
        /// </summary>
        /// <param name="rules">The rules for this <see cref="Grammar"/>.</param>
        /// <param name="settings">A collection of <see cref="KeyValuePair{TKey, TValue}"/> to be used as the settings for the compiler.</param>
        /// <param name="end">The ending cursor for this <see cref="Grammar"/>.</param>
        public Grammar(IEnumerable<Rule> rules, IEnumerable<KeyValuePair<Identifier, object>> settings, Cursor end)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            this.Rules = rules.ToList().AsReadOnly();
            this.Settings = (settings ?? Enumerable.Empty<KeyValuePair<Identifier, object>>()).ToList().AsReadOnly();
            this.End = end;
        }

        /// <summary>
        /// Gets the ending cursor for this <see cref="Grammar"/>.
        /// </summary>
        public Cursor End { get; }

        /// <summary>
        /// Gets the rules for this <see cref="Grammar"/>.
        /// </summary>
        public IList<Rule> Rules { get; }

        /// <summary>
        /// Gets the settings for this <see cref="Grammar"/>.
        /// </summary>
        public IList<KeyValuePair<Identifier, object>> Settings { get; }
    }
}
