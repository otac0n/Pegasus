// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class IdentifierTests
    {
        [Test]
        public void Constructor_WhenGivenANullEndCursor_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new Identifier("OK", start, null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenANullExpression_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new Identifier(null, start, end), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenANullStartCursor_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new Identifier("OK", null, end), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
