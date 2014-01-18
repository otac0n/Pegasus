// -----------------------------------------------------------------------
// <copyright file="PegCompilerTests.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Compiler;
    using Pegasus.Parser;

    public class PegCompilerTests
    {
        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(2, 1)]
        public void Compile_ImpossibleQuantifier_YieldsWarning(int min, int max)
        {
            var grammar = new PegParser().Parse("a = 'OK'<" + min + "," + max + ">;");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.First();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0015"));
            Assert.That(error.IsWarning, Is.True);
        }

        [Test]
        public void Compile_WhenTheGrammarHasAZeroWidthLexicalRule_ProducesAParserThatReturnsTheLexicalElements()
        {
            var grammar = new PegParser().Parse("a = b 'OK'; b -lexical = ;");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "b@0:0" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasLexicalRuleConsistingOfAClassExpression_ProducesAParserThatReturnsTheLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = [O];");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "a@0:1" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasLexicalRuleConsistingOfALiteralExpression_ProducesAParserThatReturnsTheLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = 'OK';");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "a@0:2" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasLexicalRuleConsistingOfANameExpression_ProducesAParserThatReturnsTheLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = b; b = 'OK';");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "a@0:2" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasLexicalRuleConsistingOfARepetitionExpression_ProducesAParserThatReturnsTheLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = [OK]+;");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<IList<string>>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "a@0:2" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasLexicalRuleConsistingOfAStateExpression_ProducesAParserThatReturnsTheLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = #STATE{};");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "a@0:0" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasLexicalRuleConsistingOfAWildcardExpression_ProducesAParserThatReturnsTheLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = .;");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "a@0:1" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasTwoLexicalRulesThatBeginAndEndOnTheSameCharacter_ProducesAParserThatReturnsBothLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = b; b -lexical = 'OK';");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "b@0:2", "a@0:2" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasTwoLexicalRulesThatBeginOnTheSameCharacter_ProducesAParserThatReturnsBothLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = b ' '; b -lexical = 'OK';");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse("OK ", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "b@0:2", "a@0:3" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasTwoLexicalRulesThatEndOnTheSameCharacter_ProducesAParserThatReturnsBothLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = ' ' b; b -lexical = 'OK';");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse(" OK", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "b@1:3", "a@0:3" }));
        }

        [Test]
        public void Compile_WhenTheGrammarHasTwoNestedLexicalRules_ProducesAParserThatReturnsBothLexicalElements()
        {
            var grammar = new PegParser().Parse("a -lexical = ' ' b ' '; b -lexical = 'OK';");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled.Code);

            IList<LexicalElement> lexicalElements;
            var result = parser.Parse(" OK ", null, out lexicalElements);

            var actual = lexicalElements.Select(e => e.Name + "@" + e.StartCursor.Location + ":" + e.EndCursor.Location).ToArray();
            Assert.That(actual, Is.EqualTo(new[] { "b@1:3", "a@0:4" }));
        }

        [Test]
        public void Compile_WithComplexLeftRecursion_Succeeds()
        {
            var parser = new PegParser();
            var grammar = parser.Parse(@"a<o> -memoize = b;
                                         b<o> -memoize = c;
                                         c<o> -memoize = a / d;
                                         d<o> = e;
                                         e<o> -memoize = f;
                                         f<o> -memoize = e / g;
                                         g<o> -memoize = g;");

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void Compile_WithDuplicateDefinition_YieldsError()
        {
            var grammar = new PegParser().Parse("a = 'a'; a = 'b';");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0002"));
            Assert.That(error.IsWarning, Is.False);
        }

        [Test]
        [TestCase("namespace")]
        [TestCase("classname")]
        public void Compile_WithDuplicateSetting_YieldsError(string settingName)
        {
            var grammar = new PegParser().Parse("@" + settingName + " OK; @" + settingName + " OK; a = 'OK';");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0005"));
            Assert.That(error.IsWarning, Is.False);
        }

        [Test]
        public void Compile_WithExpressionWhoseTypeIsNotDefined_YieldsError()
        {
            var parser = new PegParser();
            var grammar = parser.Parse("a -memoize = a;");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0019"));
            Assert.That(error.IsWarning, Is.False);
        }

        [Test]
        [TestCase("accessibility", "private")]
        public void Compile_WithInvalidSettingValue_YieldsError(string settingName, string value)
        {
            var grammar = new PegParser().Parse("@" + settingName + " {" + value + "}; a = 'OK';");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0012"));
            Assert.That(error.IsWarning, Is.False);
        }

        [Test]
        public void Compile_WithMissingRuleDefinition_YieldsError()
        {
            var grammar = new PegParser().Parse("a = b");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0003"));
            Assert.That(error.IsWarning, Is.False);
        }

        [Test]
        public void Compile_WithNoRules_YieldsError()
        {
            var grammar = new PegParser().Parse(" ");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0001"));
            Assert.That(error.IsWarning, Is.False);
        }

        [Test]
        public void Compile_WithSingleSimpleRule_Succeeds()
        {
            var grammar = new PegParser().Parse("start = 'OK'");

            var result = PegCompiler.Compile(grammar);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        [TestCase("a = a;")]
        [TestCase("a = '' a;")]
        [TestCase("a = b a; b = '';")]
        [TestCase("a = ('OK' / '') a;")]
        [TestCase("a = b; b = c; c = d; d = a;")]
        [TestCase("a = b / c; b = 'OK'; c = a;")]
        [TestCase("a = !b a; b = 'OK';")]
        [TestCase("a = &b c; b = a; c = 'OK';")]
        [TestCase("a = b* a; b = 'OK';")]
        [TestCase("a = ''<2,> a;")]
        public void Compile_WithUnmemoizedLeftRecursion_YieldsError(string subject)
        {
            var parser = new PegParser();
            var grammar = parser.Parse(subject);

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.First();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0020"));
            Assert.That(error.IsWarning, Is.False);
        }

        [Test]
        public void Compile_WithUnrecognizedSetting_YieldsWarning()
        {
            var grammar = new PegParser().Parse("@barnacle OK; a = 'OK';");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.First();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0006"));
            Assert.That(error.IsWarning, Is.True);
        }

        [Test]
        public void Compile_WithUnusedRules_YieldsWarning()
        {
            var grammar = new PegParser().Parse("a = b; b = 'OK'; c = d; d = 'OK' c;");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.First();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0017"));
            Assert.That(error.IsWarning, Is.True);
        }
    }
}
