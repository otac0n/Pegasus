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

    /// <summary>
    /// Represents a full set of grammar rules.
    /// </summary>
    public class Grammar
    {
        private readonly string initializer;
        private readonly IList<Rule> rules;
        private readonly IList<KeyValuePair<Identifier, string>> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grammar"/> class.
        /// </summary>
        /// <param name="rules">The rules for this <see cref="Grammar"/>.</param>
        /// <param name="settings">A collection of <see cref="KeyValuePair{TKey, TValue}"/> to be used as the settings for the compiler.</param>
        /// <param name="initializer">A section of code to be emitted at the top of the generated compiler.</param>
        public Grammar(IEnumerable<Rule> rules, IEnumerable<KeyValuePair<Identifier, string>> settings, string initializer)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            this.rules = rules.ToList().AsReadOnly();
            this.settings = (settings ?? Enumerable.Empty<KeyValuePair<Identifier, string>>()).ToList().AsReadOnly();
            this.initializer = initializer;
        }

        /// <summary>
        /// Gets the initializer for this <see cref="Grammar"/>.
        /// </summary>
        public string Initializer
        {
            get { return this.initializer; }
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
