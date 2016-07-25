// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Expressions;

    [TestFixture]
    public class ClassExpressionTests
    {
        [Theory]
        public void Constructor_WhenGivenANullCollectionOfRanges_ThrowsException(bool negated, bool ignoreCase)
        {
            Assert.That(() => new ClassExpression(null, negated, ignoreCase), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
