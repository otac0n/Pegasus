// -----------------------------------------------------------------------
// <copyright file="Grammar.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Parser;

    /// <summary>
    /// Represents a full set of grammar rules.
    /// </summary>
    public class Grammar
    {
        private readonly Cursor end;
        private readonly IList<Rule> rules;
        private readonly IList<KeyValuePair<Identifier, string>> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grammar"/> class.
        /// </summary>
        /// <param name="rules">The rules for this <see cref="Grammar"/>.</param>
        /// <param name="settings">A collection of <see cref="KeyValuePair{TKey, TValue}"/> to be used as the settings for the compiler.</param>
        /// <param name="end">The ending cursor for this <see cref="Grammar"/>.</param>
        public Grammar(IEnumerable<Rule> rules, IEnumerable<KeyValuePair<Identifier, string>> settings, Cursor end)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            this.rules = rules.ToList().AsReadOnly();
            this.settings = (settings ?? Enumerable.Empty<KeyValuePair<Identifier, string>>()).ToList().AsReadOnly();
            this.end = end;
        }

        /// <summary>
        /// Gets the ending cursor for this <see cref="Grammar"/>.
        /// </summary>
        public Cursor End
        {
            get { return this.end; }
        }

        /// <summary>
        /// Gets the rules for this <see cref="Grammar"/>.
        /// </summary>
        public IList<Rule> Rules
        {
            get { return this.rules; }
        }

        /// <summary>
        /// Gets the settings for this <see cref="Grammar"/>.
        /// </summary>
        public IList<KeyValuePair<Identifier, string>> Settings
        {
            get { return this.settings; }
        }
    }
}
