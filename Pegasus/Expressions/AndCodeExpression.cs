// -----------------------------------------------------------------------
// <copyright file="AndCodeExpression.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    /// <summary>
    /// Represents an assertion.
    /// </summary>
    public class AndCodeExpression : Expression
    {
        private readonly string code;

        /// <summary>
        /// Initializes a new instance of the <see cref="AndCodeExpression"/>.
        /// </summary>
        /// <param name="code">The code to execute for the assertion.</param>
        public AndCodeExpression(string code)
        {
            this.code = code;
        }

        /// <summary>
        /// Gets the code expression to be used as an assertion.
        /// </summary>
        public string Code
        {
            get { return this.code; }
        }
    }
}
