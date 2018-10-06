// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Describles the publicly visible rules for a grammar.
    /// </summary>
    public class VisibleRules
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleRules"/> class.
        /// </summary>
        /// <param name="startRule">The starting rule. This will be exposed via the <c>Parse</c> method.</param>
        /// <param name="publicRules">The public rules. These will be exposed via <c>ParseRuleName</c> methods.</param>
        /// <param name="exportedRules">The exported rules. These will be exposed via the <c>Exported</c> collection.</param>
        public VisibleRules(Rule startRule, List<Rule> publicRules, List<Rule> exportedRules)
        {
            this.StartRule = startRule;
            this.PublicRules = publicRules.ToList().AsReadOnly();
            this.ExportedRules = exportedRules.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the exported rules.
        /// </summary>
        /// <remarks>
        /// These will be exposed via the <c>Exported</c> collection.
        /// </remarks>
        public IList<Rule> ExportedRules { get; }

        /// <summary>
        /// Gets the public rules.
        /// </summary>
        /// <remarks>
        /// These will be exposed via <c>ParseRuleName</c> methods.
        /// </remarks>
        public IList<Rule> PublicRules { get; }

        /// <summary>
        /// Gets the starting rule.
        /// </summary>
        /// <remarks>
        /// This will be exposed via the <c>Parse</c> method.
        /// </remarks>
        public Rule StartRule { get; }
    }
}
