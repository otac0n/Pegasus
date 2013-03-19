// -----------------------------------------------------------------------
// <copyright file="ReportStartRuleNotFoundPass.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportStartRuleNotFoundPass : CompilePass
    {
        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0003" }; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001" }; }
        }

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
                        result.AddError(setting.Key.Start, () => Resources.PEG0003_RULE_DOES_NOT_EXIST, name);
                    }
                }
            }
        }
    }
}
