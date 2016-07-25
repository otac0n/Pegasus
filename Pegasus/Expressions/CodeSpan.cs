// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Expressions
{
    using System;
    using Pegasus.Common;

    /// <summary>
    /// Tracks the contents and region of a code expression.
    /// </summary>
    public class CodeSpan
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeSpan"/> class.
        /// </summary>
        /// <param name="code">The string contents of the code span.</param>
        /// <param name="start">The start of the code region.</param>
        /// <param name="end">The end of the code region.</param>
        /// <param name="value">The value of the code span.</param>
        public CodeSpan(string code, Cursor start, Cursor end, string value = null)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            this.Code = code;
            this.Start = start;
            this.End = end;
            this.value = value ?? code;
        }

        /// <summary>
        /// Gets the contents of the code span.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the start of the code region.
        /// </summary>
        public Cursor End { get; }

        /// <summary>
        /// Gets the end of the code region.
        /// </summary>
        public Cursor Start { get; }

        /// <summary>
        /// Returns the string value of this <see cref="CodeSpan"/>.
        /// </summary>
        /// <returns>The string value of this <see cref="CodeSpan"/>.</returns>
        public override string ToString() => this.value;
    }
}
