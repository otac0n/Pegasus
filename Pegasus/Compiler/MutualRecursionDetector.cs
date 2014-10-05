// -----------------------------------------------------------------------
// <copyright file="MutualRecursionDetector.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Provides mutual left-recursion detection services for Pegasus <see cref="Grammar">Grammars</see>.
    /// </summary>
    public static class MutualRecursionDetector
    {
        /// <summary>
        /// Detects which rules in a <see cref="Grammar"/> are mutually left-recursive.
        /// </summary>
        /// <param name="leftAdjacentExpressions">The set of left-adjacent <see cref="Expression">expressions</see> to inspect.</param>
        /// <returns>A <see cref="HashSet{T}"/> containing the mutually left-recursive rules.</returns>
        public static HashSet<Rule> Detect(ILookup<Rule, Expression> leftAdjacentExpressions)
        {
            var ruleLookup = leftAdjacentExpressions.ToDictionary(i => i.Key.Identifier.Name, i => i.Key);
            var mutuallyRecursive = new HashSet<Rule>();

            var index = 0;
            var stack = new Stack<Rule>();
            var ruleData = new Dictionary<Rule, RuleData>();

            Func<Rule, RuleData> strongConnect = null;
            strongConnect = v =>
            {
                var vData = ruleData[v] = new RuleData
                {
                    Index = index,
                    LowLink = index,
                };

                index++;
                stack.Push(v);

                foreach (var w in leftAdjacentExpressions[v]
                                    .OfType<NameExpression>()
                                    .Select(n => n.Identifier.Name)
                                    .Where(n => n != v.Identifier.Name)
                                    .Select(n => ruleLookup[n]))
                {
                    RuleData wData;
                    if (!ruleData.TryGetValue(w, out wData))
                    {
                        wData = strongConnect(w);
                        vData.LowLink = Math.Min(vData.LowLink, wData.LowLink);
                    }
                    else if (stack.Contains(w))
                    {
                        vData.LowLink = Math.Min(vData.LowLink, wData.Index);
                    }
                }

                if (vData.LowLink == vData.Index)
                {
                    var components = new List<Rule>();

                    Rule w;
                    do
                    {
                        w = stack.Pop();
                        components.Add(w);
                    }
                    while (w != v);

                    if (components.Count > 1)
                    {
                        mutuallyRecursive.UnionWith(components);
                    }
                }

                return vData;
            };

            foreach (var v in ruleLookup.Values)
            {
                if (!ruleData.ContainsKey(v))
                {
                    strongConnect(v);
                }
            }

            return mutuallyRecursive;
        }

        private class RuleData
        {
            public int Index { get; set; }

            public int LowLink { get; set; }
        }
    }
}
