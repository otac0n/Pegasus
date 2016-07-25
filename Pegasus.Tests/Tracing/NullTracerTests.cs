// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Tracing
{
    using System.IO;
    using NUnit.Framework;
    using Pegasus.Common.Tracing;
    using Pegasus.Compiler;
    using Pegasus.Parser;

    [TestFixture]
    public class NullTracerTests
    {
        [Test]
        public void RegressionTest()
        {
            var grammar = new PegParser().Parse(File.ReadAllText(@"Tracing\tracing-test.peg"));
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled);

            parser.Tracer = NullTracer.Instance;
            parser.Parse(File.ReadAllText(@"Tracing\tracing-test.txt"));

            Assert.Pass();
        }
    }
}
