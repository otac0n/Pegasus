// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class CodeSpanTests
    {
        [Test]
        public void Constructor_WhenGivenNullCode_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new CodeSpan(null, start, end), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenNullEndCursor_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new CodeSpan("OK", start, null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenNullStartCursor_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new CodeSpan("OK", null, end), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void ToString_WhenConstructedWithANonNullValue_ReturnsTheValue()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);
            var codeSpan = new CodeSpan("OK", start, end, "value");

            Assert.That(codeSpan.ToString(), Is.EqualTo("value"));
        }

        [Test]
        public void ToString_WhenConstructedWithANullValue_ReturnsTheCodeVerbatim()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);
            var codeSpan = new CodeSpan("OK", start, end);

            Assert.That(codeSpan.ToString(), Is.EqualTo("OK"));
        }
    }
}
