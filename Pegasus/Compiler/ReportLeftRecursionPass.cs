// -----------------------------------------------------------------------
// <copyright file="ReportLeftRecursionPass.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportLeftRecursionPass : CompilePass
    {
        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001", "PEG0002", "PEG0003" }; }
        }

        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0020" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            foreach (var rule in result.LeftRecursiveRules)
            {
                if (!rule.Flags.Any(f => f.Name == "memoize"))
                {
                    result.AddError(rule.Identifier.Start, () => Resources.PEG0020_UNMEMOIZED_LEFT_RECURSION, rule.Identifier.Name);
                }
            }
        }
    }
}
