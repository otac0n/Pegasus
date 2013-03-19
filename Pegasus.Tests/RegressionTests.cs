// -----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
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
        [Test(Description = "GitHub bug #20")]
        public void Compile_WhenGivenAGrammarWithARuleWithAnImmediateTypedExpression_DoesNotThrowAnExceptionOrReturnErrors()
        {
            var grammar = new PegParser().Parse("top = item:(<string> 'a' 'b') {item}");

            var result = PegCompiler.Compile(grammar);
            Assert.That(result.Errors, Is.Empty);
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
    }
}
