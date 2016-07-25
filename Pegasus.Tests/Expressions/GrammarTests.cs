// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class GrammarTests
    {
        [Test]
        public void Constructor_WhenGivenANullCollectionOfRules_ThrowsException()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new Grammar(null, new Dictionary<Identifier, object>(), end), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenANullEndCursor_ThrowsException()
        {
            Assert.That(() => new Grammar(new Rule[0], new Dictionary<Identifier, object>(), null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_WhenGivenANullSettingsCollection_DoesNotThrow()
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);

            Assert.That(() => new Grammar(new Rule[0], null, end), Throws.Nothing);
        }
    }
}
