// -----------------------------------------------------------------------
// <copyright file="ReportUnknownTypesPass.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportUnknownTypesPass : CompilePass
    {
        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001", "PEG0002", "PEG0003" }; }
        }

        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0019" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            var types = result.ExpressionTypes;

            foreach (var rule in grammar.Rules)
            {
                if (!types.ContainsKey(rule.Expression))
                {
                    result.AddCompilerError(rule.Identifier.Start, () => Resources.PEG0019_ERROR_UnknownType, rule.Identifier.Name);
                }
            }
        }
    }
}
