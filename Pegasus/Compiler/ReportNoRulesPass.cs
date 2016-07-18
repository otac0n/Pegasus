// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportNoRulesPass : CompilePass
    {
        public override IList<string> BlockedByErrors => new string[0];

        public override IList<string> ErrorsProduced => new[] { "PEG0001" };

        public override void Run(Grammar grammar, CompileResult result)
        {
            if (grammar.Rules.Count == 0)
            {
                var cursor = grammar.End;
                result.AddCompilerError(cursor, () => Resources.PEG0001_ERROR_NoRulesDefined);
            }
        }
    }
}
