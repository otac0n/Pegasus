// -----------------------------------------------------------------------
// <copyright file="CompilePegGrammar.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System.CodeDom.Compiler;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Provides compilation services for PEG grammars as an MSBuild task.
    /// </summary>
    public class CompilePegGrammar : Task
    {
        /// <summary>
        /// Gets or sets the filename of a file containing a grammar in PEG-format.
        /// </summary>
        [Required]
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the output filename that will contain the resulting code.
        /// </summary>
        /// <remarks>
        /// Set to null to use the default, which is the input filename with ".cs" appended.
        /// </remarks>
        public string OutputFile { get; set; }

        /// <summary>
        /// Reads and compiles the specified grammar.
        /// </summary>
        /// <returns>true, if the compilation was successful; false, otherwise.</returns>
        public override bool Execute()
        {
            CompileManager.CompileFile(this.InputFile, this.OutputFile, this.LogError);
            return !this.Log.HasLoggedErrors;
        }

        private void LogError(CompilerError error)
        {
            if (error.IsWarning)
            {
                this.Log.LogWarning(null, error.ErrorNumber, null, error.FileName, error.Line, error.Column, 0, 0, error.ErrorText);
            }
            else
            {
                this.Log.LogError(null, error.ErrorNumber, null, error.FileName, error.Line, error.Column, 0, 0, error.ErrorText);
            }
        }
    }
}
