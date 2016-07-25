// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;

    /// <summary>
    /// Represents an assertion.
    /// </summary>
    public class AndCodeExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndCodeExpression"/> class.
        /// </summary>
        /// <param name="code">The code to execute for the assertion.</param>
        public AndCodeExpression(CodeSpan code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            this.Code = code;
        }

        /// <summary>
        /// Gets the code expression to be used as an assertion.
        /// </summary>
        public CodeSpan Code { get; }
    }
}
