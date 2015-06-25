// -----------------------------------------------------------------------
// <copyright file="AndCodeExpression.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    /// <summary>
    /// Represents an assertion.
    /// </summary>
    public class AndCodeExpression : Expression
    {
        private readonly CodeSpan code;

        /// <summary>
        /// Initializes a new instance of the <see cref="AndCodeExpression"/> class.
        /// </summary>
        /// <param name="code">The code to execute for the assertion.</param>
        public AndCodeExpression(CodeSpan code)
        {
            this.code = code;
        }

        /// <summary>
        /// Gets the code expression to be used as an assertion.
        /// </summary>
        public CodeSpan Code
        {
            get { return this.code; }
        }
    }
}
