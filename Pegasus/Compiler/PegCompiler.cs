// -----------------------------------------------------------------------
// <copyright file="PegCompiler.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Provides error checking and compilation services for PEG grammars.
    /// </summary>
    public class PegCompiler
    {
        private readonly CompilePass[][] passes =
        {
            new CompilePass[]
            {
                new ReportSettingsIssuesPass(),
                new ReportNoRulesPass(),
            },
            new CompilePass[]
            {
                new ReportMissingRulesPass(),
                new ReportDuplicateRulesPass(),
                new ReportConflictingNamesPass(),
            },
            new CompilePass[]
            {
                new ReportLeftRecursionPass(),
            },
            new CompilePass[]
            {
                new GenerateCodePass(),
            },
        };

        /// <summary>
        /// Compiles a PEG grammar into a program.
        /// </summary>
        /// <param name="grammar">The grammar to compile.</param>
        /// <returns>A <see cref="CompileResult"/> containing the errors or results of compilation.</returns>
        public CompileResult Compile(Grammar grammar)
        {
            var result = new CompileResult();

            foreach (var passSet in this.passes)
            {
                foreach (var pass in passSet)
                {
                    pass.Run(grammar, result);
                }

                if (result.Errors.Any(e => !e.IsWarning))
                {
                    break;
                }
            }

            return result;
        }
    }
}
