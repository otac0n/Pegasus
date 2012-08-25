// -----------------------------------------------------------------------
// <copyright file="ReportNoRulesPass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.CodeDom.Compiler;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportNoRulesPass : CompilePass
    {
        public override void Run(Grammar grammar, CompileResult result)
        {
            if (grammar.Rules.Count == 0)
            {
                var cursor = grammar.End;
                result.Errors.Add(
                    new CompilerError(cursor.FileName, 0, 0, "PEG0001", Resources.PEG0001_NO_RULES_DEFINED));
            }
        }
    }
}
