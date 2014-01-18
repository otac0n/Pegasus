// -----------------------------------------------------------------------
// <copyright file="ReportDuplicateRulesPass.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportDuplicateRulesPass : CompilePass
    {
        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001" }; }
        }

        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0002" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            var knownRules = new HashSet<string>();

            foreach (var rule in grammar.Rules)
            {
                if (!knownRules.Add(rule.Identifier.Name))
                {
                    var cursor = rule.Identifier.Start;
                    result.AddError(cursor, () => Resources.PEG0002_RULE_ALREADY_DEFINED, rule.Identifier.Name);
                }
            }
        }
    }
}
