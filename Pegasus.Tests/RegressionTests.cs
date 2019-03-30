// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CSharp;
    using NUnit.Framework;
    using Pegasus.Compiler;
    using Pegasus.Parser;
    using static PerformanceTestUtils;

    [TestFixture]
    public class RegressionTests
    {
        [Test(Description = "GitHub bug #31")]
        public void Compile_WhenARuleContainsAStateExpressionAsPartOfASequence_IncludesTheContentOfThatStateExpression()
        {
            var grammar = new PegParser().Parse("foo = #{TEST;} 'OK';");

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Code, Contains.Substring("TEST"));
        }

        [Test(Description = "GitHub bug #31")]
        public void Compile_WhenARuleContainsAStateExpressionAsPartOfASequenceThatEndsWithACodeExpression_IncludesTheContentOfTheCodeExpression()
        {
            var grammar = new PegParser().Parse("foo = #{OK;} a:'OK' {TEST};");

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

            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled);

            Assert.That(parser.Parse("OK"), Is.EqualTo("OK"));
        }

        [Test(Description = "GitHub bug #58")]
        public void Compile_WhenErrorExpressionIsRepeated_YieldsNoErrors()
        {
            var grammar = new PegParser().Parse("a = (#error{ \"\" })*");

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
            var grammar = new PegParser().Parse(StringUtilities.JoinLines(
                "@members",
                "{",
                "    private int callCount = 0;",
                "}",
                "start <int>",
                "  = 'OK'           t:test 'NO' { t }",
                "  / 'OK' #{ } t:test      { t }",
                "test <int> -memoize = { ++callCount }"));

            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<int>(compiled);

            Assert.That(parser.Parse("OK"), Is.EqualTo(1));
        }

        [Test(Description = "GitHub bug #35")]
        public void Compile_WhenMemoizedRuleIsVisitedAfterAMutation_DoesNotUseTheMemoizedValue()
        {
            var grammar = new PegParser().Parse(StringUtilities.JoinLines(
                "@members",
                "{",
                "    private int callCount = 0;",
                "}",
                "start <int>",
                "  = 'OK'                                     t:test 'NO' { t }",
                "  / 'OK' #{ state[\"foo\"] = \"bar\"; } t:test      { t }",
                "test <int> -memoize = { ++callCount }"));

            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<int>(compiled);

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

        [Test(Description = "GitHub bug #109")]
        [TestCase("object = '' .+")]
        [TestCase("object -memoize = '' .+")]
        [TestCase("x = object; object -memoize = '' .+")]
        [TestCase("x = object:object { @object }; object = '' .+")]
        public void Compile_WhenRuleNameIsAReservedWord_CompilesCorrectly(string grammarText)
        {
            var grammar = new PegParser().Parse(grammarText);

            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled);

            Assert.That(parser.Parse("OK"), Is.EqualTo("OK"));
        }

        [Test(Description = "GitHub bug #38")]
        public void Parse_WhenARepetitionDelimiterFollowsTheRepeatedRule_DoesNotConsumeTheDelimiter()
        {
            var grammar = new PegParser().Parse("start = ' ' 'hoge'<1,,' '> ' ';");

            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled);

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

        [Test(Description = "GitHub bugs #95")]
        public void Parse_WhenTheLastElementInASequenceIsAParseExpression_DoesNotThrow()
        {
            var grammar = new PegParser().Parse("DefArg = WSO '$' v:[0-9] #parse{ DefArg(v) }; WSO = ;");

            Assert.That(() => PegCompiler.Compile(grammar), Throws.Nothing);
        }

        [Test(Description = "GitHUb bug #97")]
        [TestCase("mi nelci do")]
        [TestCase("mi nelci lonu djica lonu klama")]
        [TestCase("la .alis. co'a tatpi lo nu zutse lo rirxe korbi re'o lo mensi gi'e zukte fi no da")]
        public void Parse_WhenUsingLojbhan_DoesntTimeOut(string subject)
        {
            var parserSource = File.ReadAllText(@"TestCases\LojbanGrammar.peg");
            var grammar = new PegParser().Parse(parserSource, @"TestCases\LojbanGrammar.peg");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<object>(compiled);

            Assert.That(parser.Parse(subject), Is.EqualTo(subject));

            Evaluate(() =>
            {
                var x = parser.Parse(subject);
            });
        }

        [Test(Description = "GitHub bug #61")]
        [TestCase("foo = (#{ state[\"ok\"] = false; } ('x' #{ state[\"ok\"] = true; })* &{ state[\"ok\"] })*;")]
        [TestCase(@"EOF = !.; EOL = '\n'; line = !EOF (!EOL .)* (EOL / EOF); lines = line*;")]
        public void Parse_WhenZeroWidthRepetitionIsBlockedByAssertions_YieldsNoErrors(string grammarText)
        {
            var grammar = new PegParser().Parse(grammarText);

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Errors.Where(e => !e.IsWarning).Select(e => e.ErrorText), Is.Empty);
        }

        [Test(Description = "Ensure that the readme is accurate in several cultures.")]
        [TestCase("US")]
        [TestCase("FR")]
        [TestCase("IR")]
        [TestCase("CN")]
        [TestCase("RU")]
        public void Compile_WhenGivenTheGrammarAndExampleInTheReadme_YieldsTheExpectedOutput(string culture)
        {
            var readmeText = typeof(RegressionTests).Assembly.GetResourceString("readme.md");

            List<string> GetMarkdownCodeSections(string markdown)
            {
                const string CodePrefix = "    ";
                var sections = new List<string>();
                var lines = markdown.SplitLines();
                for (var i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(CodePrefix))
                    {
                        var j = i + 1;

                        for (; j < lines.Length; j++)
                        {
                            var line = lines[j];
                            if (!(string.IsNullOrWhiteSpace(line) || line.StartsWith(CodePrefix)))
                            {
                                break;
                            }
                        }

                        var count = j - i;
                        var nextLine = j - 1;

                        var section = new StringBuilder();
                        for (j = 0; j < count; j++)
                        {
                            var line = lines[i + j];
                            section.AppendLine(line.Length < CodePrefix.Length
                                ? string.Empty
                                : line.Substring(CodePrefix.Length));
                        }

                        sections.Add(section.ToString());

                        i = nextLine;
                    }
                }

                return sections;
            }

            var codeSections = GetMarkdownCodeSections(readmeText);
            Assume.That(codeSections.Count, Is.GreaterThanOrEqualTo(2));
            var grammarText = string.Join(Environment.NewLine, codeSections.Where(s => s.Contains("@classname")).Single());
            var programText = string.Join(Environment.NewLine, codeSections.Where(s => s.Contains("parser.Parse")).Single());
            var expectedOutput = Regex.Match(programText, @"// Outputs ""([^\r\n""]+)""").Groups[1].Value;
            Assume.That(expectedOutput, Is.Not.EqualTo(string.Empty));
            var grammar = new PegParser().Parse(grammarText);
            var compiled = PegCompiler.Compile(grammar);

            var compiler = new CSharpCodeProvider();
            var options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add(typeof(Pegasus.Common.Cursor).Assembly.Location);
            var testProgram = StringUtilities.JoinLines(
                "using System;",
                "using System.Text;",
                "public static class TestHelper",
                "{",
                "    public static string Main()",
                "    {",
                "        var Console = new TestInvariantWriter();",
                programText,
                "        return Console.ToString();",
                "    }",
                "    private class TestInvariantWriter",
                "    {",
                "        private StringBuilder sb = new StringBuilder();",
                "        public void WriteLine(object obj) { this.sb.AppendLine(Convert.ToString(obj, System.Globalization.CultureInfo.InvariantCulture)); }",
                "        public override string ToString() { return this.sb.ToString(); }",
                "    }",
                "}");

            var results = compiler.CompileAssemblyFromSource(options, compiled.Code, testProgram);
            if (results.Errors.HasErrors)
            {
                throw new CodeCompileFailedException(results.Errors.Cast<CompilerError>().ToArray(), results.Output.Cast<string>().ToArray());
            }

            CultureUtilities.WithCulture(culture, () =>
            {
                var output = (string)results.CompiledAssembly.GetType("TestHelper").GetMethod("Main").Invoke(null, new object[0]);
                Assert.That(output.TrimEnd(), Is.EqualTo(expectedOutput));
            });
        }
    }
}
