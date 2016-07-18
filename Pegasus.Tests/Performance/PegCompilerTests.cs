// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Performance
{
    using System.IO;
    using Pegasus.Compiler;
    using Pegasus.Expressions;
    using Pegasus.Parser;

    public class PegCompilerTests : PerformanceTestBase
    {
        private readonly Grammar pegGrammar;

        public PegCompilerTests()
        {
            this.pegGrammar = new PegParser().Parse(File.ReadAllText("PegParser.peg"));
        }

        [Evaluate]
        public void PegGrammar()
        {
            PegCompiler.Compile(this.pegGrammar);
        }
    }
}
