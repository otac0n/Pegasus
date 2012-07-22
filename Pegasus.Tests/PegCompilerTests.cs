// -----------------------------------------------------------------------
// <copyright file="PegCompilerTests.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using Pegasus.Compiler;
    using Pegasus.Expressions;

    public class PegCompilerTests
    {
        [Test]
        public void Compile_WithSingleSimpleRule_Succeeds()
        {
            var grammar = new Grammar(new[]
            {
                new Rule("start", new LiteralExpression("OK", ignoreCase: false)),
            });
            var compiler = new PegCompiler();

            var result = compiler.Compile(grammar);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void Compile_WithNoRules_YieldsError()
        {
            var grammar = new Grammar(Enumerable.Empty<Rule>());
            var compiler = new PegCompiler();

            var result = compiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0001"));
        }

        [Test]
        public void Compile_WithDuplicateDefinition_YieldsError()
        {
            var grammar = new Grammar(new[]
            {
                new Rule("a", new LiteralExpression("a", ignoreCase: false)),
                new Rule("a", new LiteralExpression("b", ignoreCase: false)),
            });
            var compiler = new PegCompiler();

            var result = compiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0002"));
        }

        [Test]
        public void Compile_WithMissingRuleDefinition_YieldsError()
        {
            var grammar = new Grammar(new[]
            {
                new Rule("a", new NameExpression("b")),
            });
            var compiler = new PegCompiler();

            var result = compiler.Compile(grammar);

            var error = result.Errors.Single();
            Assert.That(error.ErrorNumber, Is.EqualTo("PEG0003"));
        }
    }
}
