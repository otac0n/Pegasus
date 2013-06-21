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
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Caching;
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
                { @"^ whitespace \b", TokenType.WhiteSpace },
                { @"^ (settingName|ruleFlag|actionType) \b", TokenType.Keyword },
                { @"^ (dot|lbracket|rbracket) \s type \b ", TokenType.Delimiter },
                { @"^ (string|literal|class|dot) \b", TokenType.String },
                { @"^ identifier \b", TokenType.Identifier },
                { @"^ singleLineComment \b", TokenType.LineComment },
                { @"^ multiLineComment \b", TokenType.Comment },
                { @"^ code \b", TokenType.Text },
                { @"^ (slash|and|not|question|star|plus|lparen|rparen|equals|lt|gt|colon|semicolon|comma) \b", TokenType.Delimiter },
            }).AsReadOnly();

            private static IDictionary<TokenType, TokenColor> colorMap = new Dictionary<TokenType, TokenColor>
            {
                { TokenType.Comment, TokenColor.Comment },
                { TokenType.Delimiter, TokenColor.Text },
                { TokenType.Identifier, TokenColor.Identifier },
                { TokenType.Keyword, TokenColor.Keyword },
                { TokenType.LineComment, TokenColor.Comment },
                { TokenType.Literal, TokenColor.Number },
                { TokenType.String, TokenColor.String },
                { TokenType.Text, TokenColor.Text },
                { TokenType.WhiteSpace, TokenColor.Text },
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
                if (this.offset == this.source.Length)
                {
                    return false;
                }

                var text = GetAllText(this.buffer);

                // Some constants.
                var startIndex = state;
                var lineStartIndex = startIndex - this.offset;
                var lineEndIndex = lineStartIndex + this.source.Length;

                var tokens = GetHighlightedTokens(text);
                var token = (from l in tokens
                             where l.Item2 > startIndex
                             where l.Item1 < lineEndIndex
                             select l).FirstOrDefault();

                if (token == null)
                {
                    tokenInfo.StartIndex = this.offset;
                    tokenInfo.EndIndex = this.source.Length - 1;
                    tokenInfo.Type = TokenType.Unknown;
                    tokenInfo.Color = TokenColor.Text;
                }
                else if (token.Item1 > startIndex)
                {
                    tokenInfo.StartIndex = startIndex - lineStartIndex;
                    tokenInfo.EndIndex = token.Item1 - lineStartIndex - 1;
                    tokenInfo.Type = TokenType.Unknown;
                    tokenInfo.Color = TokenColor.Text;
                }
                else
                {
                    tokenInfo.StartIndex = Math.Max(lineStartIndex, token.Item1) - lineStartIndex;
                    tokenInfo.EndIndex = Math.Min(lineEndIndex, token.Item2) - lineStartIndex - 1;
                    tokenInfo.Type = token.Item3;
                    tokenInfo.Color = colorMap[token.Item3];
                }

                if (tokens.Count > 0 && tokenInfo.Type == TokenType.Unknown)
                {
                    Debug.WriteLine("The text '{0}' resulted in an unknown token.", this.source.Substring(tokenInfo.StartIndex, tokenInfo.EndIndex - tokenInfo.StartIndex + 1));
                }

                var tokenWidth = tokenInfo.EndIndex - tokenInfo.StartIndex + 1;
                this.offset += tokenWidth;
                state += tokenWidth;

                if (this.offset == this.source.Length)
                {
                    while (true)
                    {
                        if (state >= text.Length)
                        {
                            break;
                        }

                        var c = text[state];

                        if (c != '\r' && c != '\n')
                        {
                            break;
                        }

                        state++;
                    }
                }

                return true;
            }

            public void SetSource(string source, int offset)
            {
                this.source = source;
                this.offset = offset;
            }

            private static IList<Tuple<int, int, TokenType>> GetHighlightedTokens(string text)
            {
                var cached = MemoryCache.Default.Get(text) as IList<Tuple<int, int, TokenType>>;
                if (cached != null)
                {
                    return cached;
                }

                // Parse the text to its (possibly overlapping) lexical elements.
                var lexicalElements = GetLexicalElements(text);

                // Highlight the elements.
                var highlightedElements = HighlightLexicalElements(lexicalElements);

                // Reduce the elements to strictly non-overlapping tokens.
                var simplifiedTokens = SimplifyHighlighting(highlightedElements);

                MemoryCache.Default.Add(text, simplifiedTokens, DateTimeOffset.Now.AddMinutes(1));
                return simplifiedTokens;
            }

            private static IList<LexicalElement> GetLexicalElements(string text)
            {
                try
                {
                    IList<LexicalElement> lexicalElements;
                    new PegParser().Parse(text, null, out lexicalElements);
                    return lexicalElements;
                }
                catch (FormatException)
                {
                    return new LexicalElement[0];
                }
            }

            private static IList<Tuple<int, int, TokenType>> HighlightLexicalElements(IList<LexicalElement> lexicalElements)
            {
                var highlighted = new List<Tuple<int, int, TokenType>>(lexicalElements.Count);

                var lexicalStack = new Stack<Tuple<int, LexicalElement>>();
                foreach (var e in lexicalElements.Reverse())
                {
                    int? maxRule = null;

                    while (true)
                    {
                        if (lexicalStack.Count == 0)
                        {
                            break;
                        }

                        var top = lexicalStack.Peek();
                        if (e.EndCursor.Location <= top.Item2.EndCursor.Location && e.StartCursor.Location >= top.Item2.StartCursor.Location)
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

                    lexicalStack.Push(Tuple.Create(default(int), e));
                    var key = string.Join(" ", lexicalStack.Select(d => d.Item2.Name));
                    lexicalStack.Pop();

                    var result = Highlight(key, maxRule);

                    if (result != null)
                    {
                        lexicalStack.Push(Tuple.Create(result.Item1, e));
                        highlighted.Add(Tuple.Create(e.StartCursor.Location, e.EndCursor.Location, result.Item2));
                    }
                }

                return highlighted.AsReadOnly();
            }

            private static IList<Tuple<int, int, TokenType>> SimplifyHighlighting(IList<Tuple<int, int, TokenType>> tokens)
            {
                var simplified = new List<Tuple<int, int, TokenType>>(tokens.Count);

                var lexicalStack = new Stack<Tuple<int, int, TokenType>>();
                foreach (var t in tokens)
                {
                    while (true)
                    {
                        if (lexicalStack.Count == 0)
                        {
                            break;
                        }

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

                    lexicalStack.Push(t);
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
