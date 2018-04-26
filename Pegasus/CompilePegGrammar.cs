// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus
{
    using System.CodeDom.Compiler;
    using System.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Properties;

    /// <summary>
    /// Provides compilation services for PEG grammars as an MSBuild task.
    /// </summary>
    public class CompilePegGrammar : Task
    {
        /// <summary>
        /// Gets or sets the filenames containing grammars in PEG-format.
        /// </summary>
        [Required]
        public string[] InputFiles { get; set; }

        /// <summary>
        /// Gets or sets the output filenames that will contain the resulting code.
        /// </summary>
        /// <remarks>
        /// Set to null to use the default, which is the input filenames with ".g.cs" appended.
        /// </remarks>
        public string[] OutputFiles { get; set; }

        /// <summary>
        /// Reads and compiles the specified grammars.
        /// </summary>
        /// <returns>true, if the compilation was successful; false, otherwise.</returns>
        public override bool Execute()
        {
            var inputs = this.InputFiles.ToList();
            var outputs = (this.OutputFiles ?? new string[inputs.Count]).ToList();

            if (inputs.Count != outputs.Count)
            {
                this.Log.LogError(Resources.PEG0027_ERROR_WrongNumberOfOutputFiles);
                return false;
            }

            for (var i = 0; i < this.InputFiles.Length; i++)
            {
                CompileManager.CompileFile(inputs[i], outputs[i], this.LogError);
            }

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
