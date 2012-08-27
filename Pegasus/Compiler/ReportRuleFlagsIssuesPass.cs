// -----------------------------------------------------------------------
// <copyright file="ReportRuleFlagsIssuesPass.cs" company="(none)">
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

    internal class ReportRuleFlagsIssuesPass : CompilePass
    {
        private static readonly HashSet<string> KnownFlags = new HashSet<string>
        {
            "memoize",
        };

        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0013" }; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            foreach (var rule in grammar.Rules)
            {
                foreach (var flag in rule.Flags)
                {
                    if (!KnownFlags.Contains(flag.Name))
                    {
                        var cursor = flag.Start;
                        result.Errors.Add(
                            new CompilerError(cursor.FileName, cursor.Line, cursor.Column, "PEG0013", string.Format(Resources.PEG0013_FLAG_UNKNOWN, flag.Name)) { IsWarning = true });
                    }
                }
            }
        }
    }
}
