// -----------------------------------------------------------------------
// <copyright file="CompileResult.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>
    /// Encapsulates the results of and errors from compilation of a PEG grammar.
    /// </summary>
    public class CompileResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompileResult"/> class.
        /// </summary>
        public CompileResult()
        {
            this.Errors = new List<CompilerError>();
        }

        /// <summary>
        /// Gets the collection of errors that occurred during compilation.
        /// </summary>
        public IList<CompilerError> Errors { get; private set; }
    }
}
