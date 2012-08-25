// -----------------------------------------------------------------------
// <copyright file="ReportDuplicateRulesPass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportDuplicateRulesPass : CompilePass
    {
        public override void Run(Grammar grammar, CompileResult result)
        {
            var knownRules = new HashSet<string>();

            foreach (var rule in grammar.Rules)
            {
                if (!knownRules.Add(rule.Identifier.Name))
                {
                    result.Errors.Add(
                        new CompilerError(string.Empty, 0, 0, "PEG0002", string.Format(Resources.PEG0002_RULE_ALREADY_DEFINED, rule.Identifier.Name)));
                }
            }
        }
    }
}
