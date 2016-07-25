// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class TypedExpressionTests
    {
        [Test]
        public void Constructor_WhenGivenANullCodeSpan_ThrowsException()
        {
            Assert.That(() => new TypedExpression(null, new WildcardExpression()), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenANullExpression_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);
            var codeSpan = new CodeSpan("OK", start, end);

            Assert.That(() => new TypedExpression(codeSpan, null), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
