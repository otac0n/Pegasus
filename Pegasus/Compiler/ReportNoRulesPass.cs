// -----------------------------------------------------------------------
// <copyright file="ReportNoRulesPass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportNoRulesPass : CompilePass
    {
        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0001" }; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new string[0]; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            if (grammar.Rules.Count == 0)
            {
                var cursor = grammar.End;
                result.AddError(cursor, () => Resources.PEG0001_NO_RULES_DEFINED);
            }
        }
    }
}
