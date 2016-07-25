// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Expressions;

    [TestFixture]
    public class NameExpressionTests
    {
        [Test]
        public void Constructor_WhenGivenANullIdentifier_ThrowsException()
        {
            Assert.That(() => new NameExpression(null), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
