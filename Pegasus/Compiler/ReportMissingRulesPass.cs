// -----------------------------------------------------------------------
// <copyright file="ReportMissingRulesPass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportMissingRulesPass : CompilePass
    {
        public override void Run(Grammar grammar, CompileResult result)
        {
            var knownRules = new HashSet<string>(grammar.Rules.Select(r => r.Name));

            CompilePass.WalkExpressions(grammar, e =>
            {
                var nameExp = e as NameExpression;
                if (nameExp != null)
                {
                    if (!knownRules.Contains(nameExp.Name))
                    {
                        result.Errors.Add(
                            new CompilerError(string.Empty, 0, 0, "PEG0003", string.Format(Resources.PEG0003_RULE_DOES_NOT_EXIST, nameExp.Name)));
                    }
                }
            });
        }
    }
}
