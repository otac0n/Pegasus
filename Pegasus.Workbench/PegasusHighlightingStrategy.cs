// -----------------------------------------------------------------------
// <copyright file="PegasusHighlightingStrategy.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Text.RegularExpressions;
    using ICSharpCode.TextEditor;
    using ICSharpCode.TextEditor.Document;
    using Pegasus.Common;
    using Pegasus.Parser;

    public class PegasusHighlightingStrategy : IHighlightingStrategy
    {
        private static readonly IList<HighlightRule<string>> highlightRules = (new HighlightRuleList<string>
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
        }).AsReadOnly();

        private readonly Dictionary<string, HighlightColor> environmentColors;
        private readonly Dictionary<string, string> properties;

        public PegasusHighlightingStrategy()
        {
            this.properties = new Dictionary<string, string>();
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

        public string[] Extensions
        {
            get { return new[] { "peg", "pegasus" }; }
        }

        public string Name
        {
            get { return "Pegasus"; }
        }

        public Dictionary<string, string> Properties
        {
            get { return this.properties; }
        }

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

        public void MarkTokens(IDocument document, List<LineSegment> lines)
        {
            this.MarkTokens(document);
        }

        private static IList<HighlightedSegment<string>> GetHighlightedTokens(string text)
        {
            var cached = MemoryCache.Default.Get(text) as IList<HighlightedSegment<string>>;
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

        private static Tuple<int, string> Highlight(string key, int? maxRule = null)
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

        private static IList<HighlightedSegment<string>> HighlightLexicalElements(IList<LexicalElement> lexicalElements)
        {
            if (lexicalElements.Count == 0)
            {
                return new HighlightedSegment<string>[0];
            }

            var highlighted = new List<HighlightedSegment<string>>(lexicalElements.Count);

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

                lexicalStack.Push(Tuple.Create(maxRule ?? highlightRules.Count, e));
                var key = string.Join(" ", lexicalStack.Select(d => d.Item2.Name));
                var result = Highlight(key, maxRule);

                if (result != null)
                {
                    lexicalStack.Pop();
                    lexicalStack.Push(Tuple.Create(result.Item1, e));
                    highlighted.Add(new HighlightedSegment<string>(e.StartCursor.Location, e.EndCursor.Location, result.Item2));
                }
            }

            return highlighted.AsReadOnly();
        }

        private static IList<HighlightedSegment<string>> SimplifyHighlighting(IList<HighlightedSegment<string>> tokens)
        {
            var simplified = new List<HighlightedSegment<string>>(tokens.Count);

            var lexicalStack = new Stack<HighlightedSegment<string>>();
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
                        simplified.Add(new HighlightedSegment<string>(t.End, top.End, top.Value));
                    }

                    top = new HighlightedSegment<string>(top.Start, t.Start, top.Value);

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

        [DebuggerDisplay("[{Start}, {End}) {Value}")]
        private class HighlightedSegment<T>
        {
            private int end;
            private int start;
            private T value;

            public HighlightedSegment(int start, int end, T value)
            {
                this.start = start;
                this.end = end;
                this.value = value;
            }

            public int End
            {
                get { return this.end; }
            }

            public int Start
            {
                get { return this.start; }
            }

            public T Value
            {
                get { return this.value; }
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
