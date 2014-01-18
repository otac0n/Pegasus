// -----------------------------------------------------------------------
// <copyright file="CompileResult.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
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
    using Pegasus.Common;
    using Pegasus.Expressions;

    /// <summary>
    /// Encapsulates the results and errors from the compilation of a PEG grammar.
    /// </summary>
    public class CompileResult
    {
        private readonly Lazy<Dictionary<Expression, object>> expressionTypes;
        private readonly Grammar grammar;
        private readonly Lazy<HashSet<Rule>> leftRecursiveRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileResult"/> class.
        /// </summary>
        /// <param name="grammar">The grammar to which this <see cref="CompileResult"/> pertains.</param>
        public CompileResult(Grammar grammar)
        {
            this.grammar = grammar;
            this.expressionTypes = new Lazy<Dictionary<Expression, object>>(() => ResultTypeFinder.Find(this.grammar));
            this.leftRecursiveRules = new Lazy<HashSet<Rule>>(() => LeftRecursionDetector.Detect(this.grammar));
            this.Errors = new List<CompilerError>();
        }

        /// <summary>
        /// Gets or sets the code resulting from compilation.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets the collection of errors that occurred during compilation.
        /// </summary>
        public IList<CompilerError> Errors { get; private set; }

        /// <summary>
        /// Gets a dictionary of expressions and their corresponding types.
        /// </summary>
        public Dictionary<Expression, object> ExpressionTypes
        {
            get { return this.expressionTypes.Value; }
        }

        /// <summary>
        /// Gets the collection of left-recursive rules.
        /// </summary>
        public HashSet<Rule> LeftRecursiveRules
        {
            get { return this.leftRecursiveRules.Value; }
        }

        internal void AddError(Cursor cursor, System.Linq.Expressions.Expression<Func<string>> error, params object[] args)
        {
            this.AddCompilerError(cursor, error, args, isWarning: false);
        }

        internal void AddWarning(Cursor cursor, System.Linq.Expressions.Expression<Func<string>> error, params object[] args)
        {
            this.AddCompilerError(cursor, error, args, isWarning: true);
        }

        private void AddCompilerError(Cursor cursor, System.Linq.Expressions.Expression<Func<string>> error, object[] args, bool isWarning)
        {
            var errorId = ((System.Linq.Expressions.MemberExpression)error.Body).Member.Name.Split('_')[0];
            var errorFormat = error.Compile()();
            var errorText = string.Format(CultureInfo.CurrentCulture, errorFormat, args);
            this.Errors.Add(new CompilerError(cursor.FileName, cursor.Line, cursor.Column, errorId, errorText) { IsWarning = isWarning });
        }
    }
}
