// -----------------------------------------------------------------------
// <copyright file="ReportStartRuleNotFoundPass.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportStartRuleNotFoundPass : CompilePass
    {
        public override IList<string> BlockedByErrors => new[] { "PEG0001" };

        public override IList<string> ErrorsProduced => new[] { "PEG0003" };

        public override void Run(Grammar grammar, CompileResult result)
        {
            var knownRules = new HashSet<string>(grammar.Rules.Select(r => r.Identifier.Name));

            foreach (var setting in grammar.Settings)
            {
                if (setting.Key.Name == "start")
                {
                    var name = setting.Value.ToString().Trim();
                    if (!knownRules.Contains(name))
                    {
                        result.AddCompilerError(setting.Key.Start, () => Resources.PEG0003_ERROR_RuleDoesNotExist, name);
                    }
                }
            }
        }
    }
}
