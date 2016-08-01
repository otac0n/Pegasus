// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Common.Highlighting
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
            this.list = rules.ToList();
        }

        /// <summary>
        /// Examines the specified list of tokens and produces a new list with default tokens covering any characters that were not already represented.
        /// </summary>
        /// <remarks>
        /// For performance reasons, the specified list of tokens is assumed to be in order.
        /// </remarks>
        /// <param name="tokens">The list of existing tokens to be examined.</param>
        /// <param name="subjectLength">The length of the parsed text.</param>
        /// <param name="defaultValue">The value of the tokens that will be added.</param>
        /// <returns>A new list containing all of the original tokens and new tokens, in order.</returns>
        public List<HighlightedSegment<T>> AddDefaultTokens(IList<HighlightedSegment<T>> tokens, int subjectLength, T defaultValue)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            var result = new List<HighlightedSegment<T>>();

            var prevEnd = 0;
            foreach (var token in tokens)
            {
                if (token.Start > prevEnd)
                {
                    result.Add(new HighlightedSegment<T>(prevEnd, token.Start, defaultValue));
                }

                result.Add(token);
                prevEnd = token.End;
            }

            if (prevEnd < subjectLength)
            {
                result.Add(new HighlightedSegment<T>(prevEnd, subjectLength, defaultValue));
            }

            return result;
        }

        /// <summary>
        /// Gets the list of tokens for the specified list of lexical elements.
        /// </summary>
        /// <param name="lexicalElements">The lexical elements for which to generate tokens.</param>
        /// <returns>The list of tokens for the specified list of lexical elements.</returns>
        public List<HighlightedSegment<T>> GetTokens(IList<LexicalElement> lexicalElements)
        {
            var highlightedElements = this.HighlightLexicalElements(lexicalElements);
            var simplifiedTokens = SimplifyHighlighting(highlightedElements);
            return simplifiedTokens;
        }

        /// <summary>
        /// Examines the specified list of tokens and produces a new list with any tokens that span both whitespace and non-whitespace characters split into multiple tokens.
        /// </summary>
        /// <remarks>
        /// Some editors, such as Visual Studio, use the edges of tokens as cursor stops. Splitting on whitespace maintains expected keyboard navigation shortcuts.
        /// </remarks>
        /// <param name="tokens">The list of existing tokens to be examined.</param>
        /// <param name="subject">The original parsing subject.</param>
        /// <returns>A new list of tokens containing the original tokens, with ones that span whitespace and non-whitespace characters split.</returns>
        public List<HighlightedSegment<T>> SplitOnWhiteSpace(List<HighlightedSegment<T>> tokens, string subject)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            var result = new List<HighlightedSegment<T>>();

            foreach (var token in tokens)
            {
                if (token.End - token.Start < 2)
                {
                    result.Add(token);
                }
                else
                {
                    var prevEnd = token.Start;
                    var endIndex = token.End - 1;
                    var prevIsWhitespace = char.IsWhiteSpace(subject[prevEnd]);
                    for (int i = prevEnd + 1; i < endIndex; i++)
                    {
                        var nextIsWhitespace = char.IsWhiteSpace(subject[i]);
                        if (prevIsWhitespace != nextIsWhitespace)
                        {
                            result.Add(new HighlightedSegment<T>(prevEnd, i, token.Value));
                            prevEnd = i;
                        }

                        prevIsWhitespace = nextIsWhitespace;
                    }

                    result.Add(prevEnd == token.Start
                        ? token
                        : new HighlightedSegment<T>(prevEnd, token.End, token.Value));
                }
            }

            return result;
        }

        private static List<HighlightedSegment<T>> SimplifyHighlighting(IList<HighlightedSegment<T>> tokens)
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
            return simplified;
        }

        private Tuple<int, T> Highlight(string key, int? maxRule = null)
        {
            if (maxRule.HasValue && (maxRule.Value > this.list.Count || maxRule.Value < 0))
            {
                throw new ArgumentOutOfRangeException(nameof(maxRule));
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

        private List<HighlightedSegment<T>> HighlightLexicalElements(IList<LexicalElement> lexicalElements)
        {
            var highlighted = new List<HighlightedSegment<T>>(lexicalElements.Count);

            if (lexicalElements.Count == 0)
            {
                return highlighted;
            }

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

            return highlighted;
        }
    }
}
