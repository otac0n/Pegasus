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
            private IDictionary<string, TokenType> typeMap = new Dictionary<string, TokenType>
            {
                { "settingName", TokenType.Keyword },
                { "literal", TokenType.String },
                { "string", TokenType.String },
                { "class", TokenType.String },
                { "identifier", TokenType.Identifier },
            };

            private IDictionary<string, TokenColor> colorMap = new Dictionary<string, TokenColor>
            {
                { "settingName", TokenColor.Keyword },
                { "literal", TokenColor.String },
                { "string", TokenColor.String },
                { "class", TokenColor.String },
                { "identifier", TokenColor.Identifier },
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
                    tokenInfo.Type = typeMap[token.Name];
                    tokenInfo.Color = colorMap[token.Name];
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
    }
}
