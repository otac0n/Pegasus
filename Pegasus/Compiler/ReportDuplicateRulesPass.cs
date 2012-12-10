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
    using System.Globalization;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportDuplicateRulesPass : CompilePass
    {
        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0002" }; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            var knownRules = new HashSet<string>();

            foreach (var rule in grammar.Rules)
            {
                if (!knownRules.Add(rule.Identifier.Name))
                {
                    var cursor = rule.Identifier.Start;
                    result.Errors.Add(
                        new CompilerError(cursor.FileName, cursor.Line, cursor.Column, "PEG0002", string.Format(CultureInfo.CurrentCulture, Resources.PEG0002_RULE_ALREADY_DEFINED, rule.Identifier.Name)));
                }
            }
        }
    }
}
