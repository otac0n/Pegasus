// -----------------------------------------------------------------------
// <copyright file="CompileResult.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using Pegasus.Common;

    /// <summary>
    /// Encapsulates the results and errors from the compilation of a PEG grammar.
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

        /// <summary>
        /// Gets or sets the code resulting from compilation.
        /// </summary>
        public string Code { get; set; }

        internal void AddError(Cursor cursor, Expression<Func<string>> error, params object[] args)
        {
            this.AddCompilerError(cursor, error, args, isWarning: false);
        }

        internal void AddWarning(Cursor cursor, Expression<Func<string>> error, params object[] args)
        {
            this.AddCompilerError(cursor, error, args, isWarning: true);
        }

        private void AddCompilerError(Cursor cursor, Expression<Func<string>> error, object[] args, bool isWarning)
        {
            var errorId = ((MemberExpression)error.Body).Member.Name.Split('_')[0];
            var errorFormat = error.Compile()();
            var errorText = string.Format(CultureInfo.CurrentCulture, errorFormat, args);
            this.Errors.Add(new CompilerError(cursor.FileName, cursor.Line, cursor.Column, errorId, errorText) { IsWarning = isWarning });
        }
    }
}
