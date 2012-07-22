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
        private IList<Rule> rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grammar"/> class.
        /// </summary>
        /// <param name="rules">The rules for this <see cref="Grammar"/>.</param>
        public Grammar(IEnumerable<Rule> rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            this.rules = rules.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the rules for this <see cref="Grammar"/>.
        /// </summary>
        public IList<Rule> Rules
        {
            get { return this.rules; }
        }
    }
}
