// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Performance
{
    using System.IO;
    using NUnit.Framework;
    using Pegasus.Compiler;
    using Pegasus.Expressions;
    using Pegasus.Parser;
    using static PerformanceTests;

    [TestFixture]
    [Category("Performance")]
    public class PegCompilerTests
    {
        private readonly Grammar pegGrammar;

        public PegCompilerTests()
        {
            this.pegGrammar = new PegParser().Parse(File.ReadAllText("PegParser.peg"));
        }

        [Test]
        public void PegGrammar()
        {
            Evaluate(() =>
            {
                PegCompiler.Compile(this.pegGrammar);
            });
        }
    }
}
