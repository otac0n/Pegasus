// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Expressions;

    [TestFixture]
    public class QuantifierTests
    {
        [Datapoints]
        public readonly int[] Int32Datapoints = { 0, 1, 10 };

        [Datapoints]
        public readonly int?[] NullableInt32Datapoints = { null, 0, 1, 10 };

        [Theory]
        public void Constructor_WhenGivenANullEndCursor_ThrowsException(int min, int? max, bool nullDelimiter)
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);
            var delimiter = nullDelimiter ? null : new WildcardExpression();

            Assert.That(() => new Quantifier(start, null, min, max, delimiter), Throws.InstanceOf<ArgumentNullException>());
        }

        [Theory]
        public void Constructor_WhenGivenANullStartCursor_ThrowsException(int min, int? max, bool nullDelimiter)
        {
            var start = new Cursor("OK");
            var end = start.Advance(2);
            var delimiter = nullDelimiter ? null : new WildcardExpression();

            Assert.That(() => new Quantifier(null, end, min, max, delimiter), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
