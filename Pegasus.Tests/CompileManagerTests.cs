// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class CompileManagerTests
    {
        [TestCase(null)]
        [TestCase("OK.peg.g.cs")]
        public void CompileFile_WhenGivenANullInputFileName_ThrowsArgumentNullException(string outputFile)
        {
            Assert.That(() => CompileManager.CompileFile(null, outputFile, err => { }), Throws.InstanceOf<ArgumentNullException>());
        }

        [TestCase(null)]
        [TestCase("OK.peg.g.cs")]
        public void CompileFile_WhenGivenANullLogErrorAction_ThrowsArgumentNullException(string outputFile)
        {
            Assert.That(() => CompileManager.CompileFile("OK.peg", outputFile, null), Throws.InstanceOf<ArgumentNullException>());
        }

        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(null, "OK.peg")]
        [TestCase("", "OK.peg")]
        public void CompileString_WhenGivenANullOrEmptyString_ReturnsCompileResultWithErrors(string subject, string fileName)
        {
            var result = CompileManager.CompileString(subject, fileName);
            Assert.That(result.Errors.Single().ErrorNumber, Is.EqualTo("PEG0001"));
        }

        [TestCase("start = 'OK'", null)]
        [TestCase("start = 'OK'", "OK.peg")]
        public void CompileString_WhenGivenAValidGrammar_ReturnsCompileResultWithNoErrors(string subject, string fileName)
        {
            var result = CompileManager.CompileString(subject, fileName);
            Assert.That(result.Errors, Is.Empty);
        }

        [TestCase(@"C:\Project\Foo.peg", @"C:\Project\Foo.peg.g.cs", "Foo.peg")]
        [TestCase(@"C:\Project\Foo.peg", @"C:\Project\obj\Debug\Foo.peg.g.cs", @"C:\Project\Foo.peg")]
        public void MakePragmaPath_WhenGivenVariousOutputPaths_ReturnsTheExpectedPragmaPath(string input, string output, string pragmaPath)
        {
            var result = CompileManager.MakePragmaPath(input, output);
            Assert.That(result, Is.EqualTo(pragmaPath));
        }
    }
}
