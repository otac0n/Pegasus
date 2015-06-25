// -----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using Pegasus.Compiler;
    using Pegasus.Parser;

    [TestFixture]
    public class RegressionTests
    {
        [Test(Description = "GitHub bug #31")]
        public void Compile_WhenARuleContainsAStateExpressionAsPartOfASequence_IncludesTheContentOfThatStateExpression()
        {
            var grammar = new PegParser().Parse("foo = #STATE{TEST;} 'OK';");

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Code, Contains.Substring("TEST"));
        }

        [Test(Description = "GitHub bug #31")]
        public void Compile_WhenARuleContainsAStateExpressionAsPartOfASequenceThatEndsWithACodeExpression_IncludesTheContentOfTheCodeExpression()
        {
            var grammar = new PegParser().Parse("foo = #STATE{OK;} a:'OK' {TEST};");

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Code, Contains.Substring("TEST"));
        }

        [Test(Description = "GitHub bug #40")]
        [TestCase("start = ''*")]
        [TestCase("start = (.<0,1>)*")]
        [TestCase("start = ('OK' / )*")]
        public void Compile_WhenAZeroLengthProductionIsRepeated_YieldsError(string grammarText)
        {
            var grammar = new PegParser().Parse(grammarText);

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Where(e => !e.IsWarning).Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0021"));
        }

        [Test(Description = "GitHub bug #40")]
        public void Compile_WhenAZeroLengthProductionIsRepeatedWithADelimiter_YieldsNoErrors()
        {
            var grammar = new PegParser().Parse("start = ''<0,,'OK'>");

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Errors.Where(e => !e.IsWarning).Select(e => e.ErrorText), Is.Empty);
        }

        [Test(Description = "GitHub bug #39")]
        public void Compile_WhenEmptyParensAreIncluded_CompilesCorrectly()
        {
            var grammar = new PegParser().Parse("a = () b; b = 'OK';");

            var result = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(result.Code);

            Assert.That(parser.Parse("OK"), Is.EqualTo("OK"));
        }

        [Test(Description = "GitHub bug #58")]
        public void Compile_WhenErrorExpressionIsRepeated_YieldsNoErrors()
        {
            var grammar = new PegParser().Parse("a = (#ERROR{ \"\" })*");

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Errors.Where(e => !e.IsWarning).Select(e => e.ErrorText), Is.Empty);
        }

        [Test(Description = "GitHub bug #21")]
        [TestCase("accessibility", "foo-public-foo")]
        [TestCase("accessibility", "foo-internal-foo")]
        public void Compile_WhenGivenAGrammarWithAnInvalidAccessibilitySetting_YieldsError(string settingName, string value)
        {
            var grammar = new PegParser().Parse("@" + settingName + " {" + value + "}; a = 'OK';");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0012"));
        }

        [Test(Description = "GitHub bug #20")]
        public void Compile_WhenGivenAGrammarWithARuleWithAnImmediateTypedExpression_DoesNotThrowAnExceptionOrReturnErrors()
        {
            var grammar = new PegParser().Parse("top = item:(<string> 'a' 'b') {item}");

            var result = PegCompiler.Compile(grammar);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test(Description = "GitHub bug #30")]
        public void Compile_WhenGivenAGrammarWithUnusedRules_YieldsNoErrors()
        {
            var grammar = new PegParser().Parse("i = 'OK'; unused = '';");

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Errors.Where(e => !e.IsWarning).Select(e => e.ErrorText), Is.Empty);
        }

        [Test(Description = "GitHub bug #35")]
        public void Compile_WhenMemoizedRuleIsReVisitedWithoutStateMutation_UsesTheMemoizedValue()
        {
            var grammar = new PegParser().Parse(string.Join("\n", new[] {
                "@members",
                "{",
                "    private int callCount = 0;",
                "}",
                "start <int>",
                "  = 'OK'           t:test 'NO' { t }",
                "  / 'OK' #STATE{ } t:test      { t }",
                "",
                "test <int> -memoize = { ++callCount }"
            }));

            var result = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<int>(result.Code);

            Assert.That(parser.Parse("OK"), Is.EqualTo(1));
        }

        [Test(Description = "GitHub bug #35")]
        public void Compile_WhenMemoizedRuleIsVisitedAfterAMutation_DoesNotUseTheMemoizedValue()
        {
            var grammar = new PegParser().Parse(string.Join("\n", new[] {
                "@members",
                "{",
                "    private int callCount = 0;",
                "}",
                "start <int>",
                "  = 'OK'                                     t:test 'NO' { t }",
                "  / 'OK' #STATE{ state[\"foo\"] = \"bar\"; } t:test      { t }",
                "",
                "test <int> -memoize = { ++callCount }"
            }));

            var result = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<int>(result.Code);

            Assert.That(parser.Parse("OK"), Is.EqualTo(2));
        }

        [Test]
        public void Compile_WhenRepetitionExpressionContainsDelimiterButRepetitionIsLimitedToOne_YieldsWarning()
        {
            var grammar = new PegParser().Parse("a = 'foo'<0,1,','>");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0024"));
        }

        [Test(Description = "GitHub bug #38")]
        public void Parse_WhenARepetitionDelimiterFollowsTheRepeatedRule_DoesNotConsumeTheDelimiter()
        {
            var grammar = new PegParser().Parse("start = ' ' 'hoge'<1,,' '> ' ';");

            var result = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(result.Code);

            Assert.That(parser.Parse(" hoge "), Is.EqualTo(" hoge "));
        }

        [Test(Description = "GitHub bugs #50 & #52")]
        public void Parse_WhenLeftRecursiveRulesAreNested_YieldsErrors()
        {
            var grammar = new PegParser().Parse("exp -memoize = dot / call / name; dot -memoize = exp '.' name; call -memoize = exp '(' ')'; name = '' [a-z]+;");

            var result = PegCompiler.Compile(grammar);

            var errorNumber = result.Errors.Select(e => e.ErrorNumber).Distinct().Single();
            Assert.That(errorNumber, Is.EqualTo("PEG0023"));
        }

        [Test(Description = "GitHub bug #61")]
        [TestCase("foo = (#STATE{ state[\"ok\"] = false; } ('x' #STATE{ state[\"ok\"] = true; })* &{ state[\"ok\"] })*;")]
        [TestCase(@"EOF = !.; EOL = '\n'; line = !EOF (!EOL .)* (EOL / EOF); lines = line*;")]
        public void Parse_WhenZeroWidthRepetitionIsBlockedByAssertions_YieldsNoErrors(string grammarText)
        {
            var grammar = new PegParser().Parse(grammarText);

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Errors.Where(e => !e.IsWarning).Select(e => e.ErrorText), Is.Empty);
        }
    }
}
