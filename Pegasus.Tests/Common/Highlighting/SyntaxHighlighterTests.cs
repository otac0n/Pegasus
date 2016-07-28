// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Common.Highlighting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Common.Highlighting;
    using Pegasus.Compiler;
    using Pegasus.Parser;

    public class SyntaxHighlighterTests
    {
        public enum Token
        {
            Number,
            Text,
            Unknown,
            Whitespace,
        }

        [Test]
        public void Highlight_GivenAGrammarThatReturnsIdenticalAdjacentLexicalElements_ReturnsCombinedToken()
        {
            TestScenario(
                grammar: "text -lexical = '' .*;",
                rules: new HighlightRuleCollection<Token>
                {
                    { @"^ text \b", Token.Text },
                },
                tokens: new TokenList
                {
                    { "this", Token.Text },
                    { " ", Token.Text },
                    { "is", Token.Text },
                    { " ", Token.Text },
                    { "a", Token.Text },
                    { " ", Token.Text },
                    { "sentence.", Token.Text },
                });
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

        [Test]
        public void Highlight_WhenTheParseResultDoesNotContainLexicalElements_ReturnsDefaultToken()
        {
            TestScenario(
                grammar: "start = '' .*; fake -lexical = 'OK';",
                rules: new HighlightRuleCollection<Token>(),
                tokens: new TokenList
                {
                    { "asdf", Token.Unknown },
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
            var parser = CodeCompiler.Compile<string>(compiled);

            IList<LexicalElement> lexicalElements;
            parser.Parse(subject, null, out lexicalElements);

            var highlighter = new SyntaxHighlighter<Token>(rules);
            var tokens = highlighter.GetTokens(lexicalElements);
            tokens = SyntaxHighlighter<Token>.AddDefaultTokens(tokens, subject.Length, Token.Unknown);
            tokens = SyntaxHighlighter<Token>.SplitOnWhiteSpace(tokens, subject);

            Assert.That(ToAssertString(tokens), Is.EqualTo(ToAssertString(result)));
        }

        private static string ToAssertString(IEnumerable<HighlightedSegment<Token>> segments) => StringUtilities.JoinLines(segments.Select(s => $"({s.Start}:{s.End}:{s.Value})"));

        private class SegmentList : List<HighlightedSegment<Token>>
        {
            public void Add(int start, int end, Token token)
            {
                this.Add(new HighlightedSegment<Token>(start, end, token));
            }
        }

        private class TokenList : List<Tuple<string, Token>>
        {
            public void Add(string value, Token token)
            {
                this.Add(Tuple.Create(value, token));
            }
        }
    }
}
