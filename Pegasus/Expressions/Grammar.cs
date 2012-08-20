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
        private readonly IList<KeyValuePair<string, string>> settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grammar"/> class.
        /// </summary>
        /// <param name="rules">The rules for this <see cref="Grammar"/>.</param>
        public Grammar(IEnumerable<Rule> rules, IEnumerable<KeyValuePair<string, string>> settings, string initializer)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            this.rules = rules.ToList().AsReadOnly();
            this.settings = (settings ?? Enumerable.Empty<KeyValuePair<string, string>>()).ToList().AsReadOnly();
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
        public IList<KeyValuePair<string, string>> Settings
        {
            get { return this.settings; }
        }
    }
}
