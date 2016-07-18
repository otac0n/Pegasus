// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportRuleFlagsIssuesPass : CompilePass
    {
        private static readonly HashSet<string> KnownFlags = new HashSet<string>
        {
            "memoize",
            "lexical",
            "public",
            "export",
        };

        public override IList<string> BlockedByErrors => new[] { "PEG0001" };

        public override IList<string> ErrorsProduced => new[] { "PEG0013" };

        public override void Run(Grammar grammar, CompileResult result)
        {
            foreach (var rule in grammar.Rules)
            {
                foreach (var flag in rule.Flags)
                {
                    if (!KnownFlags.Contains(flag.Name))
                    {
                        var cursor = flag.Start;
                        result.AddCompilerError(cursor, () => Resources.PEG0013_WARNING_FlagUnknown, flag.Name);
                    }
                }
            }
        }
    }
}
