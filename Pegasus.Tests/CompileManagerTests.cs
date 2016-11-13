// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class CompileManagerTests
    {
        [Test]
        public void CompileFile_WhenGivenANullInputFileName_ThrowsArgumentNullException()
        {
            Assert.That(() => CompileManager.CompileFile(null, "OK", err => { }), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void CompileFile_WhenGivenANullLogErrorAction_ThrowsArgumentNullException()
        {
            Assert.That(() => CompileManager.CompileFile("OK.peg", "OK", null), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
