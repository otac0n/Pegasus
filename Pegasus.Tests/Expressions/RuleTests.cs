// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class RuleTests
    {
        [Test]
        public void Constructor_WhenGivenANullExpression_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new Rule(new Identifier("OK", start, end), null, new Identifier[0]), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenANullFlagsCollection_DoesNotThrow()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new Rule(new Identifier("OK", start, end), new WildcardExpression(), null), Throws.Nothing);
        }

        [Test]
        public void Constructor_WhenGivenANullIdentifier_ThrowsException()
        {
            Assert.That(() => new Rule(null, new WildcardExpression(), new Identifier[0]), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
