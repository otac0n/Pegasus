// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Expressions;

    [TestFixture]
    public class ChoiceExpressionTests
    {
        [Test]
        public void Constructor_WhenGivenANullCollectionOfChoices_ThrowsException()
        {
            Assert.That(() => new ChoiceExpression(null), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
