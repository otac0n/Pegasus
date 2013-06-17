// -----------------------------------------------------------------------
// <copyright file="PegasusLanguageService.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Package
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Package;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Pegasus.Common;
    using Pegasus.Parser;

    /// <summary>
    /// Implements a <see cref="LanguageService"/> for Pegasus grammars.
    /// </summary>
    public class PegasusLanguageService : LanguageService
    {
        private static LanguagePreferences preferences = new LanguagePreferences();

        /// <summary>
        /// Gets the name of the language service.
        /// </summary>
        public override string Name
        {
            get { return "Pegasus"; }
        }

        /// <summary>
        /// Returns a list of file extension filters suitable for a Save As dialog box.
        /// </summary>
        /// <returns>If successful, returns a string containing the file extension filters; otherwise, returns an empty string.</returns>
        public override string GetFormatFilterList()
        {
            return "Pegasus Grammars (*.peg)|*.peg";
        }

        /// <summary>
        /// Returns a <see cref="LanguagePreferences"/> object for this language service.
        /// </summary>
        /// <returns>If successful, returns a <see cref="LanguagePreferences"/> object; otherwise, returns a null value.</returns>
        public override LanguagePreferences GetLanguagePreferences()
        {
            return preferences;
        }

        /// <summary>
        /// Returns a single instantiation of a parser.
        /// </summary>
        /// <param name="buffer">An <see cref="IVsTextLines"/> representing the lines of source to parse.</param>
        /// <returns>If successful, returns an <see cref="IScanner"/> object; otherwise, returns a null value.</returns>
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return new Scanner(buffer);
        }

        /// <summary>
        /// Parses the source based on the specified <see cref="ParseRequest"/> object.
        /// </summary>
        /// <param name="req">The <see cref="ParseRequest"/> describing how to parse the source file.</param>
        /// <returns>If successful, returns an <see cref="AuthoringScope"/> object; otherwise, returns a null value.</returns>
        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return null;
        }

        private class Scanner : IScanner
        {
            private static IList<HighlightRule<TokenType>> highlightRules = (new HighlightRuleList<TokenType>
            {
                { @"^ whitespace       \b", TokenType.WhiteSpace },
                { @"^ settingName      \b", TokenType.Keyword    },
                { @"^ (string|literal) \b", TokenType.String     },
                { @"^ class            \b", TokenType.String     },
                { @"^ identifier       \b", TokenType.Identifier },
            }).AsReadOnly();

            private static IDictionary<TokenType, TokenColor> colorMap = new Dictionary<TokenType, TokenColor>
            {
                { TokenType.Comment,    TokenColor.Comment    },
                { TokenType.Identifier, TokenColor.Identifier },
                { TokenType.Keyword,    TokenColor.Keyword    },
                { TokenType.Literal,    TokenColor.Number     },
                { TokenType.String,     TokenColor.String     },
                { TokenType.Text,       TokenColor.Text       },
                { TokenType.WhiteSpace, TokenColor.Text       },
            };

            private IVsTextLines buffer;
            private string source;
            private int offset;

            public Scanner(IVsTextLines buffer)
            {
                this.buffer = buffer;
            }

            public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
            {
                var text = GetAllText(this.buffer);

                // Some constants.
                var startIndex = state;
                var lineStartIndex = startIndex - this.offset;
                var lineEndIndex = lineStartIndex + this.source.Length;

                if (this.offset == this.source.Length)
                {
                    return false;
                }

                LexicalElement token;

                try
                {
                    var lexicalElements = GetLexicalElements(text);
                    var tokens = GetHighlightedTokens(text);

                    token = (from l in lexicalElements
                             where l.EndCursor.Location > startIndex
                             where l.StartCursor.Location < lineEndIndex
                             select l).FirstOrDefault();
                }
                catch
                {
                    token = null;
                }

                if (token == null)
                {
                    tokenInfo.StartIndex = this.offset;
                    tokenInfo.EndIndex = this.source.Length - 1;
                    tokenInfo.Type = TokenType.Unknown;
                    tokenInfo.Color = TokenColor.Text;
                }
                else
                {
                    tokenInfo.StartIndex = Math.Max(lineStartIndex, token.StartCursor.Location) - lineStartIndex;
                    tokenInfo.EndIndex = Math.Min(lineEndIndex, token.EndCursor.Location) - lineStartIndex - 1;
                    tokenInfo.Type = TokenType.Unknown;
                    tokenInfo.Color = TokenColor.Text;
                }

                var tokenWidth = tokenInfo.EndIndex - tokenInfo.StartIndex + 1;
                this.offset += tokenWidth;
                state += tokenWidth;

                if (this.offset != this.source.Length)
                {
                    return true;
                }

                while (true)
                {
                    if (state >= text.Length) break;

                    var c = text[state];

                    if (c != '\r' && c != '\n') break;

                    state++;
                }

                return false;
            }

            private static IList<LexicalElement> GetLexicalElements(string text)
            {
                var lexicalElements = (IList<LexicalElement>)System.Runtime.Caching.MemoryCache.Default.Get(text);
                if (lexicalElements == null)
                {
                    var parsed = new PegParser().Parse(text, null, out lexicalElements);
                    System.Runtime.Caching.MemoryCache.Default.Add(text, lexicalElements, DateTimeOffset.Now.AddMinutes(1));
                }

                return lexicalElements;
            }

            private static IList<Tuple<int, int, TokenType>> GetHighlightedTokens(string text)
            {
                // Parse the text to its (possibly overlapping) lexical elements.
                var lexicalElements = GetLexicalElements(text);

                // Highlight the elements.
                var highlightedElements = HighlightLexicalElements(lexicalElements);

                // Reduce the elements to strictly non-overlapping tokens.
                var simplifiedTokens = SimplifyHighlighting(highlightedElements);

                return simplifiedTokens;
            }

            private static IList<Tuple<int, int, Tuple<int, TokenType>>> HighlightLexicalElements(IList<LexicalElement> lexicalElements)
            {
                var highlighted = new List<Tuple<int, int, Tuple<int, TokenType>>>(lexicalElements.Count);

                var lexicalStack = new Stack<Tuple<int?, LexicalElement>>();
                foreach (var e in lexicalElements.Reverse())
                {
                    int? maxRule = null;

                    while (true)
                    {
                        if (lexicalStack.Count == 0) break;
                        var top = lexicalStack.Peek();
                        maxRule = top.Item1;
                        if (e.EndCursor.Location <= top.Item2.EndCursor.Location && e.StartCursor.Location >= top.Item2.StartCursor.Location) break;
                        lexicalStack.Pop();
                    }

                    lexicalStack.Push(Tuple.Create(maxRule, e));

                    var key = string.Join(" ", lexicalStack.Select(d => d.Item2.Name));

                    var result = Highlight(key);

                    if (result != null)
                    {
                        lexicalStack.Pop();
                        lexicalStack.Push(Tuple.Create((int?)result.Item1, e));
                        highlighted.Add(Tuple.Create(e.StartCursor.Location, e.EndCursor.Location, Tuple.Create(result.Item1, result.Item2)));
                    }
                }

                return highlighted.AsReadOnly();
            }

            private static IList<Tuple<int, int, TokenType>> SimplifyHighlighting(IList<Tuple<int, int, Tuple<int, TokenType>>> tokens)
            {
                var simplified = new List<Tuple<int, int, TokenType>>(tokens.Count);

                var lexicalStack = new Stack<Tuple<int, int, TokenType>>();
                foreach (var t in tokens)
                {
                    while (true)
                    {
                        if (lexicalStack.Count == 0) break;

                        var top = lexicalStack.Pop();
                        if (top.Item1 >= t.Item2)
                        {
                            simplified.Add(top);
                            continue;
                        }

                        if (top.Item2 > t.Item2)
                        {
                            simplified.Add(Tuple.Create(t.Item2, top.Item2, top.Item3));
                        }

                        top = Tuple.Create(top.Item1, t.Item1, top.Item3);

                        if (top.Item1 < top.Item2)
                        {
                            lexicalStack.Push(top);
                            break;
                        }
                    }

                    lexicalStack.Push(Tuple.Create(t.Item1, t.Item2, t.Item3.Item2));
                }

                while (lexicalStack.Count > 0)
                {
                    simplified.Add(lexicalStack.Pop());
                }

                simplified.Reverse();
                return simplified.AsReadOnly();
            }

            private static Tuple<int, TokenType> Highlight(string key, int? maxRule = null)
            {
                if (maxRule.HasValue && (maxRule.Value > highlightRules.Count || maxRule.Value < 0))
                {
                    throw new ArgumentOutOfRangeException("maxRule");
                }

                maxRule = maxRule ?? highlightRules.Count;

                for (var i = 0; i < maxRule.Value; i++)
                {
                    var rule = highlightRules[i];

                    if (rule.Pattern.IsMatch(key))
                    {
                        return Tuple.Create(i, rule.Value);
                    }
                }

                return null;
            }

            public void SetSource(string source, int offset)
            {
                this.source = source;
                this.offset = offset;
            }

            private static string GetAllText(IVsTextLines buffer)
            {
                int endLine, endIndex;
                string text;

                if (buffer.GetLastLineIndex(out endLine, out endIndex) != VSConstants.S_OK ||
                    buffer.GetLineText(0, 0, endLine, endIndex, out text) != VSConstants.S_OK)
                {
                    text = null;
                }

                return text;
            }
        }

        private class HighlightRule<T>
        {
            private Regex pattern;
            private T value;

            public HighlightRule(string pattern, T value)
            {
                this.pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                this.value = value;
            }

            public Regex Pattern
            {
                get { return this.pattern; }
            }

            public T Value
            {
                get { return this.value; }
            }
        }

        private class HighlightRuleList<T> : List<HighlightRule<T>>
        {
            public void Add(string pattern, T value)
            {
                this.Add(new HighlightRule<T>(pattern, value));
            }
        }
    }
}
