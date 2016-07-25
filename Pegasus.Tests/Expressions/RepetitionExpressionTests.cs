// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class RepetitionExpressionTests
    {
        [Test]
        public void Constructor_WhenGivenANullExpression_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);
            var quantifier = new Quantifier(start, end, 0);

            Assert.That(() => new RepetitionExpression(null, quantifier), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenANullQuantifier_ThrowsException()
        {
            Assert.That(() => new RepetitionExpression(new WildcardExpression(), null), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
