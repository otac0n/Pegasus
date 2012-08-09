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
    /// Represents a code expression to be emitted in the source code of the generated parser.
    /// </summary>
    public class CodeExpression : Expression
    {
        private readonly string code;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExpression"/> class.
        /// </summary>
        /// <param name="ranges">The ranges that match.</param>
        /// <param name="negated">A value indicating whether or not the expression is negated.</param>
        /// <param name="ignoreCase">A value indicating whether or not the expression should ignore case differences when matching.</param>
        public CodeExpression(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            this.code = code;
        }

        /// <summary>
        /// Gets the code that this expression contains.
        /// </summary>
        public string Code
        {
            get { return this.code; }
        }
    }
}
