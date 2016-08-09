// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Parser
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;
    using Pegasus.Parser;
    using static PerformanceTestUtils;

    public class PegParserTests
    {
        [Test]
        [Category("Performance")]
        public void Parse_Performance_NestedEmptyGrammar()
        {
            var emptyGrammar = "a = ((())) ((() (() (((()) (() () ()))) ()) () (((((() (( ()) () (( ())))) (() (()))) () () (())) () (() (() () (())) ()) () () (((( ()) ())) ((()) (()) ) () () (((()) ) ()) ) ((() (() ()) ((()) ())))) ) ()) (((() ((() ((()) ()) ())) (() (() (()) () ((())))))) (((() ()))) (() ((() (()) () (()) (() ()))) ()) ()) () ()) ()";

            Evaluate(() =>
            {
                new PegParser().Parse(emptyGrammar);
            });
        }

        [Test]
        [Category("Performance")]
        public void Parse_Performance_PegGrammar()
        {
            var pegGrammar = File.ReadAllText("PegParser.peg");

            Evaluate(() =>
            {
                new PegParser().Parse(pegGrammar);
            });
        }

        [Test]
        public void Parse_WhenACSharpExpressionDoesntConsumeAllOfTheSourceText_YieldsError()
        {
            var parser = new PegParser();

            try
            {
                parser.Parse("a = {{ return \"OK\"; } extra }");
            }
            catch (FormatException ex)
            {
                Assert.That(ex.Message, Is.StringStarting("PEG0011:"));
                var cursor = (Cursor)ex.Data["cursor"];
                Assert.That(cursor.Location, Is.EqualTo(22));
            }
        }

        [Test]
        public void Parse_WhenACSharpExpressionHasUnbalancedCurlyBraces_Succeeds()
        {
            var parser = new PegParser();
            Assert.That(() => parser.Parse("curly = { \"{\" }"), Throws.Nothing);
        }

        [TestCase("a = (('' ('') (())) (('' '') '' ''))")]
        [TestCase("a = ('' '' ('' () () '') '' (('') ''))")]
        [TestCase("a = '' ('' ((('' '')))) '' () '' '' ''")]
        [TestCase("a = (((()) '' '') () ('' '') (() '' ()))")]
        public void Parse_WhenTheGrammarIsEntirelyEmpty_ReturnsAnEmptySequence(string subject)
        {
            var parser = new PegParser();

            var grammar = parser.Parse(subject);

            var result = (SequenceExpression)grammar.Rules.Single().Expression;
            Assert.That(result.Sequence, Is.Empty);
        }

        [Test]
        [TestCase("a = [abc]i", true)]
        [TestCase("a = [abc]s", false)]
        [TestCase("a = [abc]", null)]
        public void Parse_WithClassExpression_YieldsClassExpressionWithCorrectCaseSensitivity(string subject, bool? ignoreCase)
        {
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var charClass = (ClassExpression)grammar.Rules.Single().Expression;

            Assert.That(charClass.IgnoreCase, Is.EqualTo(ignoreCase));
        }

        [Test]
        public void Parse_WithClassExpression_YieldsClassExpressionWithCorrectCharacterRanges()
        {
            var subject = "a = [-a-z0-9]";
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var charClass = (ClassExpression)grammar.Rules.Single().Expression;

            var expected = new[]
            {
                new CharacterRange('-', '-'),
                new CharacterRange('a', 'z'),
                new CharacterRange('0', '9'),
            };
            Assert.That(charClass.Ranges, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCase("a = #ERROR{ \"OK\" }", CodeType.Error, false)]
        [TestCase("a = #error{ \"OK\" }", CodeType.Error, false)]
        [TestCase("a = #STATE{}", CodeType.State, false)]
        [TestCase("a = #state{}", CodeType.State, false)]
        [TestCase("a = #{}", CodeType.State, false)]
        [TestCase("a = #PARSE{ null }", CodeType.Parse, false)]
        [TestCase("a = #parse{ null }", CodeType.Parse, false)]
        [TestCase("a = { \"OK\" }", CodeType.Result, true)]
        public void Parse_WithCodeExpression_YieldsCodeExpressionWithCorrectCodeType(string subject, CodeType codeType, bool inSequence)
        {
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var expression = grammar.Rules.Single().Expression;
            var codeExpression = (CodeExpression)(inSequence ? ((SequenceExpression)expression).Sequence.Single() : expression);

            Assert.That(codeExpression.CodeType, Is.EqualTo(codeType));
        }

        [Test]
        [TestCase("a = 'OK'i", true)]
        [TestCase("a = 'OK's", false)]
        [TestCase("a = 'OK'", null)]
        public void Parse_WithLiteralExpression_YieldsLiteralExpressionWithCorrectCaseSensitivity(string subject, bool? ignoreCase)
        {
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var literal = (LiteralExpression)grammar.Rules.Single().Expression;

            Assert.That(literal.IgnoreCase, Is.EqualTo(ignoreCase));
        }

        [Test]
        public void Parse_WithLiteralExpression_YieldsLiteralExpressionWithCorrectString()
        {
            var subject = "a = 'testString'";
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var literal = (LiteralExpression)grammar.Rules.Single().Expression;

            Assert.That(literal.Value, Is.EqualTo("testString"));
        }

        [Test]
        public void Parse_WithNoRules_YieldsEmptyGrammar()
        {
            var subject = string.Empty;
            var parser = new PegParser();

            var grammar = parser.Parse(subject);

            Assert.That(grammar.Rules, Is.Empty);
        }

        [Test]
        public void Parse_WithPegGrammar_Works()
        {
            var subject = File.ReadAllText("PegParser.peg");
            var parser = new PegParser();

            var result = parser.Parse(subject);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Parse_WithSingleEmptyRule_YieldsRuleWithEmptySequenceExpression()
        {
            var subject = "a = ";
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var sequence = (SequenceExpression)grammar.Rules.Single().Expression;

            Assert.That(sequence.Sequence, Is.Empty);
        }

        [Test]
        public void Parse_WithSingleEmptyRule_YieldsRuleWithMatchingName()
        {
            var subject = "testName = ";
            var parser = new PegParser();

            var grammar = parser.Parse(subject);

            Assert.That(grammar.Rules.Single().Identifier.Name, Is.EqualTo("testName"));
        }
    }
}
