// -----------------------------------------------------------------------
// <copyright file="CodeExpression.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Describes the semantics of a code expression.
    /// </summary>
    public enum CodeType
    {
        /// <summary>
        /// Indicates that the code should be used as a successful result.
        /// </summary>
        Result,

        /// <summary>
        /// Indicates that the code should be used to throw an error.
        /// </summary>
        Error,
    }

    /// <summary>
    /// Represents a code expression to be emitted in the source code of the generated parser.
    /// </summary>
    public class CodeExpression : Expression
    {
        private readonly string code;
        private readonly CodeType codeType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeExpression"/> class.
        /// </summary>
        /// <param name="code">The literal code to be contained by this expression.</param>
        public CodeExpression(string code, CodeType codeType)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            this.code = code;
            this.codeType = codeType;
        }

        /// <summary>
        /// Gets the code that this expression contains.
        /// </summary>
        public string Code
        {
            get { return this.code; }
        }

        /// <summary>
        /// Gets the semantic usage of this expression.
        /// </summary>
        public CodeType CodeType
        {
            get { return this.codeType; }
        }
    }
}
