// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Performance
{
    using System.IO;
    using System.Linq;
    using Compiler;
    using NUnit.Framework;
    using Pegasus.Parser;
    using static PerformanceTests;

    [TestFixture]
    [Category("Performance")]
    public class RegressionTests
    {
        [TestCase("simple")]
        [TestCase("gitter-piratejon")]
        public void GeneratedParserPerformanceRegression(string testName)
        {
            var parserSource = File.ReadAllText($@"Performance\Regression\{testName}.peg");
            var subject = File.ReadAllText($@"Performance\Regression\{testName}.txt");
            var parsed = new PegParser().Parse(parserSource);
            var compiled = PegCompiler.Compile(parsed);
            Assert.That(compiled.Errors.Where(e => !e.IsWarning), Is.Empty);
            var pegParser = CodeCompiler.Compile<dynamic>(compiled.Code);

            Evaluate(() =>
            {
                pegParser.Parse(subject);
            });
        }
    }
}
