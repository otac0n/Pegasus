// -----------------------------------------------------------------------
// <copyright file="NotCodeExpression.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    /// <summary>
    /// Represents a negative assertion.
    /// </summary>
    public class NotCodeExpression : Expression
    {
        private readonly CodeSpan code;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotCodeExpression"/> class.
        /// </summary>
        /// <param name="code">The code to execute for the negative assertion.</param>
        public NotCodeExpression(CodeSpan code)
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
