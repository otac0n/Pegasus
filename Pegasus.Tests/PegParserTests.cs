// -----------------------------------------------------------------------
// <copyright file="PegParserTests.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using Pegasus.Expressions;
    using Pegasus.Parser;

    public class PegParserTests
    {
        [Test]
        public void Parse_WithPegGrammar_Works()
        {
            var subject = File.ReadAllText("PegParser.peg");
            var parser = new PegParser();

            var result = parser.Parse(subject);
            Assert.That(result, Is.Not.Null);
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
        public void Parse_WithSingleEmptyRule_YieldsRuleWithMatchingName()
        {
            var subject = "testName = ";
            var parser = new PegParser();

            var grammar = parser.Parse(subject);

            Assert.That(grammar.Rules.Single().Identifier.Name, Is.EqualTo("testName"));
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
        public void Parse_WithLiteralExpression_YieldsLiteralExpressionWithCorrectString()
        {
            var subject = "a = 'testString'";
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var literal = (LiteralExpression)grammar.Rules.Single().Expression;

            Assert.That(literal.Value, Is.EqualTo("testString"));
        }

        [Test]
        [TestCase("a = 'OK'i", true)]
        [TestCase("a = 'OK'", false)]
        public void Parse_WithLiteralExpression_YieldsLiteralExpressionWithCorrectCaseSensitivity(string subject, bool ignoreCase)
        {
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var literal = (LiteralExpression)grammar.Rules.Single().Expression;

            Assert.That(literal.IgnoreCase, Is.EqualTo(ignoreCase));
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
        [TestCase("a = [abc]i", true)]
        [TestCase("a = [abc]", false)]
        public void Parse_WithClassExpression_YieldsClassExpressionWithCorrectCaseSensitivity(string subject, bool ignoreCase)
        {
            var parser = new PegParser();

            var grammar = parser.Parse(subject);
            var charClass = (ClassExpression)grammar.Rules.Single().Expression;

            Assert.That(charClass.IgnoreCase, Is.EqualTo(ignoreCase));
        }
    }
}
