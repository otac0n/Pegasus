// -----------------------------------------------------------------------
// <copyright file="LeftRecursionDetector.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Provides left-recursion detection services for Pegasus <see cref="Grammar">Grammars</see>.
    /// </summary>
    public static class LeftRecursionDetector
    {
        /// <summary>
        /// Detects which rules in a <see cref="Grammar"/> are left-recursive.
        /// </summary>
        /// <param name="leftAdjacentExpressions">The left-adjacent expressions to inspect.</param>
        /// <returns>A <see cref="HashSet{T}"/> containing the left-recursive rules.</returns>
        /// <remarks>This does not detect mutual left-recursion.</remarks>
        public static HashSet<Rule> Detect(ILookup<Rule, Expression> leftAdjacentExpressions)
        {
            return new HashSet<Rule>(from i in leftAdjacentExpressions
                                     where i.OfType<NameExpression>().Where(n => n.Identifier.Name == i.Key.Identifier.Name).Any()
                                     select i.Key);
        }
    }
}
