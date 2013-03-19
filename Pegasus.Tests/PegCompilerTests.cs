// -----------------------------------------------------------------------
// <copyright file="PegCompilerTests.cs" company="(none)">
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

    public class PegCompilerTests
    {
        [Test]
        public void Compile_WithSingleSimpleRule_Succeeds()
        {
            var grammar = new PegParser().Parse("start = 'OK'");

            var result = PegCompiler.Compile(grammar);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void Compile_WithNoRules_YieldsError()
        {
            var grammar = new PegParser().Parse(" ");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0001"));
        }

        [Test]
        public void Compile_WithDuplicateDefinition_YieldsError()
        {
            var grammar = new PegParser().Parse("a = 'a'; a = 'b';");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0002"));
        }

        [Test]
        public void Compile_WithMissingRuleDefinition_YieldsError()
        {
            var grammar = new PegParser().Parse("a = b");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0003"));
        }

        [Test]
        [TestCase("a = a;")]
        [TestCase("a = '' a;")]
        [TestCase("a = ('OK' / '') a;")]
        [TestCase("a = b; b = c; c = d; d = a;")]
        [TestCase("a = b / c; b = 'OK'; c = a;")]
        [TestCase("a = !b a; b = 'OK';")]
        [TestCase("a = &b c; b = a; c = 'OK';")]
        [TestCase("a = b* a; b = 'OK';")]
        public void Compile_WithLeftRecursion_YieldsError(string subject)
        {
            var parser = new PegParser();
            var grammar = parser.Parse(subject);

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0004"));
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
        }

        [Test]
        [TestCase("accessibility", "private")]
        public void Compile_WithInvalidSettingValue_YieldsError(string settingName, string value)
        {
            var grammar = new PegParser().Parse("@" + settingName + " {" + value + "}; a = 'OK';");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0012"));
        }

        [Test]
        public void Compile_WithUnrecognizedSetting_YieldsWarning()
        {
            var grammar = new PegParser().Parse("@barnacle OK; a = 'OK';");

            var result = PegCompiler.Compile(grammar);

            var error = result.Errors.First();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0006"));
        }

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
        }
    }
}
