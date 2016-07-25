// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Expressions;

    [TestFixture]
    public class NotExpressionTests
    {
        [Test]
        public void Constructor_WhenGivenANullExpression_ThrowsException()
        {
            Assert.That(() => new NotExpression(null), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
