// -----------------------------------------------------------------------
// <copyright file="NotCodeExpression.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    /// <summary>
    /// Represents a negative assertion.
    /// </summary>
    public class NotCodeExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotCodeExpression"/> class.
        /// </summary>
        /// <param name="code">The code to execute for the negative assertion.</param>
        public NotCodeExpression(CodeSpan code)
        {
            this.Code = code;
        }

        /// <summary>
        /// Gets the code expression to be used as an assertion.
        /// </summary>
        public CodeSpan Code { get; }
    }
}
