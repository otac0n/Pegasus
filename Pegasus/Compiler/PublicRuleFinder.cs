// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Contains the logic for discovering publicly visible rules.
    /// </summary>
    public static class PublicRuleFinder
    {
        /// <summary>
        /// Finds the publicly visible rules.
        /// </summary>
        /// <param name="grammar">The grammar to search.</param>
        /// <returns>An object describing the publicly visible rules for the specified grammar.</returns>
        public static VisibleRules Find(Grammar grammar)
        {
            var startRuleName = grammar.Settings.Where(s => s.Key.Name == SettingName.Start).Select(s => s.Value.ToString()).FirstOrDefault();
            var startRule = default(Rule);
            var publicRules = new List<Rule>();
            var exportedRules = new List<Rule>();
            foreach (var rule in grammar.Rules)
            {
                if (startRule == null && (startRuleName == null || rule.Identifier.Name == startRuleName))
                {
                    startRule = rule;
                }

                if (rule.Flags.Any(f => f.Name == "public"))
                {
                    publicRules.Add(rule);
                }

                if (rule.Flags.Any(f => f.Name == "export"))
                {
                    exportedRules.Add(rule);
                }
            }

            if (startRuleName == null && (publicRules.Count > 0 || exportedRules.Count > 0))
            {
                startRule = null;
            }

            return new VisibleRules(
                startRule,
                publicRules,
                exportedRules);
        }

        /// <summary>
        /// Gets the public name for the specified rule.
        /// </summary>
        /// <param name="rule">The rule whose public name will be determined.</param>
        /// <returns>The public name.</returns>
        public static string GetPublicName(Rule rule) => GetPublicName(rule.Identifier.Name);

        /// <summary>
        /// Gets the public name for the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier whose public name will be determined.</param>
        /// <returns>The public name.</returns>
        public static string GetPublicName(Identifier identifier) => GetPublicName(identifier.Name);

        /// <summary>
        /// Gets the public name for the specified rule name.
        /// </summary>
        /// <param name="name">The name to be made into a public name.</param>
        /// <returns>The public name.</returns>
        public static string GetPublicName(string name) => name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
    }
}
