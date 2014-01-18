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
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Common;

    /// <summary>
    /// Provides syntax highlighting services for Pegasus grammars.
    /// </summary>
    /// <typeparam name="T">The type of the value of each token.</typeparam>
    public class SyntaxHighlighter<T>
    {
        private readonly IList<HighlightRule<T>> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxHighlighter{T}"/> class.
        /// </summary>
        /// <param name="rules">The rules for the syntax highlighter.</param>
        public SyntaxHighlighter(IEnumerable<HighlightRule<T>> rules)
        {
            this.list = rules.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the list of tokens for the specified list of lexical elements.
        /// </summary>
        /// <param name="lexicalElements">The lexical elements for which to generate tokens.</param>
        /// <returns>The list of tokens for the specified list of lexical elements.</returns>
        public IList<HighlightedSegment<T>> GetTokens(IList<LexicalElement> lexicalElements)
        {
            var highlightedElements = this.HighlightLexicalElements(lexicalElements);
            var simplifiedTokens = SimplifyHighlighting(highlightedElements);
            return simplifiedTokens;
        }

        private static IList<HighlightedSegment<T>> SimplifyHighlighting(IList<HighlightedSegment<T>> tokens)
        {
            var simplified = new List<HighlightedSegment<T>>(tokens.Count);

            var lexicalStack = new Stack<HighlightedSegment<T>>();
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
                        simplified.Add(new HighlightedSegment<T>(t.End, top.End, top.Value));
                    }

                    top = new HighlightedSegment<T>(top.Start, t.Start, top.Value);

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

        private IList<HighlightedSegment<T>> HighlightLexicalElements(IList<LexicalElement> lexicalElements)
        {
            if (lexicalElements.Count == 0)
            {
                return new HighlightedSegment<T>[0];
            }

            var highlighted = new List<HighlightedSegment<T>>(lexicalElements.Count);

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
                    highlighted.Add(new HighlightedSegment<T>(e.StartCursor.Location, e.EndCursor.Location, result.Item2));
                }
            }

            return highlighted.AsReadOnly();
        }
    }
}
