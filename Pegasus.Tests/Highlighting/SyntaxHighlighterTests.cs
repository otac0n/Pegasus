// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Tests.Highlighting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common;
    using Compiler;
    using NUnit.Framework;
    using Parser;
    using Pegasus.Highlighting;

    public class SyntaxHighlighterTests
    {
        public enum Token
        {
            Whitespace,
            Number,
        }

        [Test]
        public void Highlight_GivenSimpleLexicalData_ReturnsHighlightedTokens()
        {
            TestScenario(
                grammar: "start = '' digits<0,,whitespace>; digits -lexical = '' [0-9]+; whitespace -lexical = '' [ ]+",
                rules: new HighlightRuleCollection<Token>
                {
                    { @"^ whitespace \b", Token.Whitespace },
                    { @"^ digits \b", Token.Number },
                },
                tokens: new TokenList
                {
                    { "123", Token.Number },
                    { " ", Token.Whitespace },
                    { "123", Token.Number },
                    { "  ", Token.Whitespace },
                    { "789", Token.Number },
                });
        }

        private static void TestScenario(string grammar, HighlightRuleCollection<Token> rules, TokenList tokens)
        {
            var result = new List<HighlightedSegment<Token>>();
            var subject = new StringBuilder();
            foreach (var token in tokens)
            {
                var start = subject.Length;
                subject.Append(token.Item1);
                var end = subject.Length;

                result.Add(new HighlightedSegment<Token>(start, end, token.Item2));
            }

            TestScenario(grammar, rules, subject.ToString(), result);
        }

        private static void TestScenario(string grammar, HighlightRuleCollection<Token> rules, string subject, IEnumerable<HighlightedSegment<Token>> result)
        {
            var compiled = PegCompiler.Compile(new PegParser().Parse(grammar));
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            parser.Parse(subject, null, out lexicalElements);

            var highlighter = new SyntaxHighlighter<Token>(rules);
            var tokens = highlighter.GetTokens(lexicalElements);

            Assert.That(ToAssertString(tokens), Is.EqualTo(ToAssertString(result)));
        }

        private static string ToAssertString(IEnumerable<HighlightedSegment<Token>> segments) => string.Join("\n", segments.Select(s => $"({s.Start}:{s.End}:{s.Value})"));

        private class TokenList : List<Tuple<string, Token>>
        {
            public void Add(string value, Token token)
            {
                this.Add(Tuple.Create(value, token));
            }
        }

        private class SegmentList : List<HighlightedSegment<Token>>
        {
            public void Add(int start, int end, Token token)
            {
                this.Add(new HighlightedSegment<Token>(start, end, token));
            }
        }
    }
}
