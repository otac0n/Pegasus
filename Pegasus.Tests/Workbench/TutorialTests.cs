// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Tests.Workbench
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Pegasus.Compiler;
    using Pegasus.Parser;
    using Pegasus.Workbench;

    [TestFixture]
    public class TutorialTests
    {
        private IEnumerable<TestCaseData> Tutorials => Tutorial.FindAll().Select(t => new TestCaseData(t));

        [TestCaseSource("Tutorials")]
        public void Compile_ForAllFoundTutorials_Succeeds(Tutorial tutorial)
        {
            var grammar = new PegParser().Parse(tutorial.GrammarText);

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Errors, Is.Empty);
        }

        [TestCaseSource("Tutorials")]
        public void Parse_ForAllFoundTutorials_Succeeds(Tutorial tutorial)
        {
            var grammar = new PegParser().Parse(tutorial.GrammarText);
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<object>(compiled.Code);

            var result = parser.Parse(tutorial.TestText);

            Assert.That(result, Is.Not.Null);
        }

        [TestCase("0", 0)]
        [TestCase("1+1", 2)]
        [TestCase("((((((1+1))))))", 2)]
        [TestCase("5.1+2*3", 11.1)]
        [TestCase("2^3^2", 512)]
        [TestCase("1+2*3^4", 163)]
        [TestCase("(3.1+2.1)", 5.2)]
        [TestCase("(1.2+4.5)/(6*3.2+(3.4-2.1*5/2))", 0.32853025936599423)]
        [TestCase("(8.1)/(9)", 0.9)]
        public void Parse_For05Calculator_ReturnsCorrectValueForMathematicalExpressions(string expression, double value)
        {
            var tutorial = Tutorial.FindAll().Single(t => t.Name == "05 - Calculator");
            var grammar = new PegParser().Parse(tutorial.GrammarText);
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<object>(compiled.Code);

            var result = parser.Parse(expression);

            Assert.That(result, Is.EqualTo(value).Within(0.1).Percent);
        }
    }
}
