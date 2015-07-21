// -----------------------------------------------------------------------
// <copyright file="CompileResult.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Pegasus.Common;
    using Pegasus.Expressions;

    /// <summary>
    /// Encapsulates the results and errors from the compilation of a PEG grammar.
    /// </summary>
    public class CompileResult
    {
        private readonly Lazy<Dictionary<Expression, object>> expressionTypes;
        private readonly Grammar grammar;
        private readonly Lazy<ILookup<Rule, Expression>> leftAdjacentExpressions;
        private readonly Lazy<HashSet<Rule>> leftRecursiveRules;
        private readonly Lazy<HashSet<Rule>> mutuallyRecursiveRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileResult"/> class.
        /// </summary>
        /// <param name="grammar">The grammar to which this <see cref="CompileResult"/> pertains.</param>
        public CompileResult(Grammar grammar)
        {
            this.grammar = grammar;
            this.expressionTypes = new Lazy<Dictionary<Expression, object>>(() => ResultTypeFinder.Find(this.grammar));
            this.leftAdjacentExpressions = new Lazy<ILookup<Rule, Expression>>(() => LeftAdjacencyDetector.Detect(this.grammar));
            this.leftRecursiveRules = new Lazy<HashSet<Rule>>(() => LeftRecursionDetector.Detect(this.LeftAdjacentExpressions));
            this.mutuallyRecursiveRules = new Lazy<HashSet<Rule>>(() => MutualRecursionDetector.Detect(this.LeftAdjacentExpressions));
            this.Errors = new List<CompilerError>();
        }

        /// <summary>
        /// Gets or sets the code resulting from compilation.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets the collection of errors that occurred during compilation.
        /// </summary>
        public IList<CompilerError> Errors { get; }

        /// <summary>
        /// Gets a dictionary of expressions and their corresponding types.
        /// </summary>
        public Dictionary<Expression, object> ExpressionTypes => this.expressionTypes.Value;

        /// <summary>
        /// Gets the collection of left-recursive rules.
        /// </summary>
        public ILookup<Rule, Expression> LeftAdjacentExpressions => this.leftAdjacentExpressions.Value;

        /// <summary>
        /// Gets the collection of left-recursive rules.
        /// </summary>
        public HashSet<Rule> LeftRecursiveRules => this.leftRecursiveRules.Value;

        /// <summary>
        /// Gets the collection of mutually left-recursive rules.
        /// </summary>
        public HashSet<Rule> MutuallyRecursiveRules => this.mutuallyRecursiveRules.Value;

        internal void AddCompilerError(Cursor cursor, System.Linq.Expressions.Expression<Func<string>> error, params object[] args)
        {
            var parts = ((System.Linq.Expressions.MemberExpression)error.Body).Member.Name.Split('_');
            var errorId = parts[0];

            bool isWarning;
            switch (parts[1])
            {
                case "ERROR":
                    isWarning = false;
                    break;

                case "WARNING":
                    isWarning = true;
                    break;

                default:
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Unknown error type '{0}'.", parts[1]), "error");
            }

            var errorFormat = error.Compile()();
            var errorText = string.Format(CultureInfo.CurrentCulture, errorFormat, args);
            this.Errors.Add(new CompilerError(cursor.FileName, cursor.Line, cursor.Column, errorId, errorText) { IsWarning = isWarning });
        }
    }
}
