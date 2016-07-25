// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class LiteralExpressionTests
    {
        [Theory]
        public void Constructor_WhenGivenANullEndCursor_ThrowsException(bool ignoreCase, bool fromResource)
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new LiteralExpression(start, null, "OK", ignoreCase, fromResource), Throws.InstanceOf<ArgumentNullException>());
        }

        [Theory]
        public void Constructor_WhenGivenANullStartCursor_ThrowsException(bool ignoreCase, bool fromResource)
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new LiteralExpression(null, end, "OK", ignoreCase, fromResource), Throws.InstanceOf<ArgumentNullException>());
        }

        [Theory]
        public void Constructor_WhenGivenANullValue_ThrowsException(bool ignoreCase, bool fromResource)
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new LiteralExpression(start, end, null, ignoreCase, fromResource), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
