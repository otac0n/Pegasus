// -----------------------------------------------------------------------
// <copyright file="PegasusHighlightingStrategy.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.Caching;
    using ICSharpCode.TextEditor;
    using ICSharpCode.TextEditor.Document;
    using Pegasus.Common;
    using Pegasus.Highlighting;
    using Pegasus.Parser;

    internal class PegasusHighlightingStrategy : IHighlightingStrategy
    {
        private static readonly SyntaxHighlighter<string> SyntaxHighlighter = new SyntaxHighlighter<string>(new HighlightRuleCollection<string>
        {
            { @"^ whitespace \b", "WhiteSpace" },
            { @"^ (settingName|ruleFlag|actionType) \b", "Keyword" },
            { @"^ (dot|lbracket|rbracket) \s type \b ", "Delimiter" },
            { @"^ (string|literal|class|dot) \b", "String" },
            { @"^ identifier \s ruleName \b", "RuleName" },
            { @"^ identifier \b", "Identifier" },
            { @"^ (singleLineComment|multiLineComment) \b", "Comment" },
            { @"^ code \b", "Text" },
            { @"^ (slash|and|not|question|star|plus|lparen|rparen|equals|lt|gt|colon|semicolon|comma) \b", "Delimiter" },
        });

        private readonly Dictionary<string, HighlightColor> environmentColors;

        public PegasusHighlightingStrategy()
        {
            this.Properties = new Dictionary<string, string>();
            this.environmentColors = new Dictionary<string, HighlightColor>
            {
                { "Default", new HighlightBackground("WindowText", "Window", false, false) },
                { "Selection", new HighlightColor("HighlightText", "Highlight", false, false) },
                { "VRuler", new HighlightColor("ControlLight", "Window", false, false) },
                { "InvalidLines", new HighlightColor(Color.Red, false, false) },
                { "CaretMarker", new HighlightColor(Color.Yellow, false, false) },
                { "CaretLine", new HighlightBackground("ControlLight", "Window", false, false) },
                { "LineNumbers", new HighlightBackground("ControlDark", "Window", false, false) },
                { "FoldLine", new HighlightColor("ControlDark", false, false) },
                { "FoldMarker", new HighlightColor("WindowText", "Window", false, false) },
                { "SelectedFoldLine", new HighlightColor("WindowText", false, false) },
                { "EOLMarkers", new HighlightColor("ControlLight", "Window", false, false) },
                { "SpaceMarkers", new HighlightColor("ControlLight", "Window", false, false) },
                { "TabMarkers", new HighlightColor("ControlLight", "Window", false, false) },
            };
        }

        public string[] Extensions => new[] { "peg", "pegasus" };

        public string Name => "Pegasus";

        public Dictionary<string, string> Properties { get; }

        public HighlightColor GetColorFor(string name)
        {
            switch (name)
            {
                case "String":
                    return new HighlightColor(Color.FromArgb(163, 21, 21), false, false);

                case "RuleName":
                    return new HighlightColor(Color.FromArgb(43, 145, 175), false, false);

                case "Keyword":
                    return new HighlightColor(Color.Blue, false, false);

                case "Comment":
                    return new HighlightColor(Color.DarkGreen, false, false);
            }

            HighlightColor color;
            if (name == null || !this.environmentColors.TryGetValue(name, out color))
            {
                color = new HighlightColor(SystemColors.WindowText, false, false);
            }

            return color;
        }

        public void MarkTokens(IDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var tokens = GetHighlightedTokens(document.TextContent);

            var tokenEnum = tokens.GetEnumerator();
            var tokenEnd = !tokenEnum.MoveNext();

            foreach (var line in document.LineSegmentCollection)
            {
                (line.Words = line.Words ?? new List<TextWord>()).Clear();

                var offset = line.Offset;

                while (!tokenEnd)
                {
                    if (offset >= line.Offset + line.Length)
                    {
                        break;
                    }

                    var token = tokenEnum.Current;
                    if (token.End <= offset)
                    {
                        tokenEnd = !tokenEnum.MoveNext();
                        continue;
                    }

                    if (offset < token.Start)
                    {
                        token = new HighlightedSegment<string>(offset, token.Start, null);
                    }

                    var end = Math.Min(line.Offset + line.Length, token.End);
                    line.Words.Add(new TextWord(document, line, offset - line.Offset, end - offset, this.GetColorFor(token.Value), true));
                    offset = end;
                }

                if (offset < line.Offset + line.Length)
                {
                    line.Words.Add(new TextWord(document, line, offset - line.Offset, line.Offset + line.Length - offset, this.GetColorFor(null), true));
                }
            }

            document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
            document.CommitUpdate();
        }

        public void MarkTokens(IDocument document, List<LineSegment> lines) => this.MarkTokens(document);

        private static IList<HighlightedSegment<string>> GetHighlightedTokens(string text)
        {
            var cached = MemoryCache.Default.Get(text) as IList<HighlightedSegment<string>>;
            if (cached != null)
            {
                return cached;
            }

            var simplifiedTokens = SyntaxHighlighter.GetTokens(GetLexicalElements(text));

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
    }
}
