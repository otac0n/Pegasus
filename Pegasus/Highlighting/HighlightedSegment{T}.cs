// -----------------------------------------------------------------------
// <copyright file="HighlightedSegment{T}.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Highlighting
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a segment of text that is highlighted with an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value of the highlighted segment.</typeparam>
    [DebuggerDisplay("[{Start}, {End}) {Value}")]
    public class HighlightedSegment<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightedSegment{T}"/> class.
        /// </summary>
        /// <param name="start">The starting index of the segment.</param>
        /// <param name="end">The ending index of the segment.</param>
        /// <param name="value">The value of the segment.</param>
        public HighlightedSegment(int start, int end, T value)
        {
            this.Start = start;
            this.End = end;
            this.Value = value;
        }

        /// <summary>
        /// Gets the ending index of the segment.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// Gets the starting index of the segment.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Gets the value of the segment.
        /// </summary>
        public T Value { get; }
    }
}
