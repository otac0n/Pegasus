// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Expressions;

    [TestFixture]
    public class SequenceExpressionTests
    {
        [Test]
        public void Constructor_WhenGivenANullSequenceCollection_ThrowsException()
        {
            Assert.That(() => new SequenceExpression(null), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
