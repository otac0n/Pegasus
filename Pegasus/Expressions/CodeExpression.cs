// -----------------------------------------------------------------------
// <copyright file="CodeExpression.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
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

        /// <summary>
        /// Indicates that the code should be used to modify the current parser state.
        /// </summary>
        State,
    }

    /// <summary>
    /// Represents a code expression to be emitted in the source code of the generated parser.
    /// </summary>
    public class CodeExpression : Expression
    {
        private readonly CodeSpan codeSpan;
        private readonly CodeType codeType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeExpression"/> class.
        /// </summary>
        /// <param name="codeSpan">The literal code to be contained by this expression.</param>
        /// <param name="codeType">The semantic usage of this expression.</param>
        public CodeExpression(CodeSpan codeSpan, CodeType codeType)
        {
            if (codeSpan == null)
            {
                throw new ArgumentNullException("codeSpan");
            }

            this.codeSpan = codeSpan;
            this.codeType = codeType;
        }

        /// <summary>
        /// Gets the code that this expression contains.
        /// </summary>
        public CodeSpan CodeSpan
        {
            get { return this.codeSpan; }
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
