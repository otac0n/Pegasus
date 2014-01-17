// -----------------------------------------------------------------------
// <copyright file="SyntaxHighlighter{T}.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Pegasus.Common;

    /// <summary>
    /// Provides syntax highlighting services for Pegasus grammars.
    /// </summary>
    /// <typeparam name="T">The type of the value of each token.</typeparam>
    public class SyntaxHighlighter<T> : IEnumerable<SyntaxHighlighter<T>.HighlightRule>
    {
        private readonly List<HighlightRule> list = new List<HighlightRule>();

        /// <summary>
        /// Adds the specified pattern to the list of highlight rules.
        /// </summary>
        /// <param name="pattern">The pattern used to match the token.</param>
        /// <param name="value">The value of the token.</param>
        public void Add(string pattern, T value)
        {
            this.list.Add(new HighlightRule(pattern, value));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator&lt;T&gt;" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<SyntaxHighlighter<T>.HighlightRule> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Gets the list of tokens for the specified list of lexical elements.
        /// </summary>
        /// <param name="lexicalElements">The lexical elements for which to generate tokens.</param>
        /// <returns>The list of tokens for the specified list of lexical elements.</returns>
        public IList<HighlightedSegment> GetTokens(IList<LexicalElement> lexicalElements)
        {
            var highlightedElements = this.HighlightLexicalElements(lexicalElements);
            var simplifiedTokens = SimplifyHighlighting(highlightedElements);
            return simplifiedTokens;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static IList<HighlightedSegment> SimplifyHighlighting(IList<HighlightedSegment> tokens)
        {
            var simplified = new List<HighlightedSegment>(tokens.Count);

            var lexicalStack = new Stack<HighlightedSegment>();
            foreach (var t in tokens)
            {
                while (true)
                {
                    if (lexicalStack.Count == 0)
                    {
                        break;
                    }

                    var top = lexicalStack.Pop();
                    if (top.Start >= t.End)
                    {
                        simplified.Add(top);
                        continue;
                    }

                    if (top.End > t.End)
                    {
                        simplified.Add(new HighlightedSegment(t.End, top.End, top.Value));
                    }

                    top = new HighlightedSegment(top.Start, t.Start, top.Value);

                    if (top.Start < top.End)
                    {
                        lexicalStack.Push(top);
                        break;
                    }
                }

                lexicalStack.Push(t);
            }

            while (lexicalStack.Count > 0)
            {
                simplified.Add(lexicalStack.Pop());
            }

            simplified.Reverse();
            return simplified.AsReadOnly();
        }

        private Tuple<int, T> Highlight(string key, int? maxRule = null)
        {
            if (maxRule.HasValue && (maxRule.Value > this.list.Count || maxRule.Value < 0))
            {
                throw new ArgumentOutOfRangeException("maxRule");
            }

            maxRule = maxRule ?? this.list.Count;

            for (var i = 0; i < maxRule.Value; i++)
            {
                var rule = this.list[i];

                if (rule.Pattern.IsMatch(key))
                {
                    return Tuple.Create(i, rule.Value);
                }
            }

            return null;
        }

        private IList<HighlightedSegment> HighlightLexicalElements(IList<LexicalElement> lexicalElements)
        {
            if (lexicalElements.Count == 0)
            {
                return new HighlightedSegment[0];
            }

            var highlighted = new List<HighlightedSegment>(lexicalElements.Count);

            var lexicalStack = new Stack<Tuple<int, LexicalElement>>();
            foreach (var e in lexicalElements.Reverse().Where(e => e.StartCursor.Location != e.EndCursor.Location))
            {
                int? maxRule = null;

                while (true)
                {
                    if (lexicalStack.Count == 0)
                    {
                        break;
                    }

                    var top = lexicalStack.Peek();
                    if (e.EndCursor.Location > top.Item2.StartCursor.Location && e.StartCursor.Location < top.Item2.EndCursor.Location)
                    {
                        maxRule = top.Item1;
                        break;
                    }

                    lexicalStack.Pop();
                }

                if (maxRule == 0)
                {
                    continue;
                }

                lexicalStack.Push(Tuple.Create(maxRule ?? this.list.Count, e));
                var key = string.Join(" ", lexicalStack.Select(d => d.Item2.Name));
                var result = this.Highlight(key, maxRule);

                if (result != null)
                {
                    lexicalStack.Pop();
                    lexicalStack.Push(Tuple.Create(result.Item1, e));
                    highlighted.Add(new HighlightedSegment(e.StartCursor.Location, e.EndCursor.Location, result.Item2));
                }
            }

            return highlighted.AsReadOnly();
        }

        /// <summary>
        /// Represents a segment of text that is highlighted with an object of type <typeparamref name="T"/>.
        /// </summary>
        [DebuggerDisplay("[{Start}, {End}) {Value}")]
        public class HighlightedSegment
        {
            private int end;
            private int start;
            private T value;

            /// <summary>
            /// Initializes a new instance of the <see cref="HighlightedSegment"/> class.
            /// </summary>
            /// <param name="start">The starting index of the segment.</param>
            /// <param name="end">The ending index of the segment.</param>
            /// <param name="value">The value of the segment.</param>
            public HighlightedSegment(int start, int end, T value)
            {
                this.start = start;
                this.end = end;
                this.value = value;
            }

            /// <summary>
            /// Gets the ending index of the segment.
            /// </summary>
            public int End
            {
                get { return this.end; }
            }

            /// <summary>
            /// Gets the starting index of the segment.
            /// </summary>
            public int Start
            {
                get { return this.start; }
            }

            /// <summary>
            /// Gets the value of the segment.
            /// </summary>
            public T Value
            {
                get { return this.value; }
            }
        }

        /// <summary>
        /// Represents a rule for highlighting.
        /// </summary>
        public class HighlightRule
        {
            private Regex pattern;
            private T value;

            /// <summary>
            /// Initializes a new instance of the <see cref="HighlightRule"/> class.
            /// </summary>
            /// <param name="pattern">The <see cref="Regex"/> pattern to use for matching.</param>
            /// <param name="value">The value of the match.</param>
            public HighlightRule(string pattern, T value)
            {
                this.pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                this.value = value;
            }

            /// <summary>
            /// Gets the pattern to use for matching.
            /// </summary>
            public Regex Pattern
            {
                get { return this.pattern; }
            }

            /// <summary>
            /// Gets the value of the match.
            /// </summary>
            public T Value
            {
                get { return this.value; }
            }
        }
    }
}
