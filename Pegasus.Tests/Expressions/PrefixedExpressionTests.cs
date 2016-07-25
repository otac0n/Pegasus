// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class PrefixedExpressionTests
    {
        [Test]
        public void Constructor_WhenGivenANullExpression_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new PrefixedExpression(new Identifier("OK", start, end), null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenANullPrefix_ThrowsException()
        {
            Assert.That(() => new PrefixedExpression(null, new WildcardExpression()), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
