// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using NUnit.Framework;
    using Pegasus.Common;

    public class ParseResultTests
    {
        [Test]
        public void Equals_WithOtherObject_ReturnsFalse()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = new ParseResult<int>(start, end, 0);
            var subjectB = new object();

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }

        [Test]
        public void OpEquality_WithBothSidesNullReference_ReturnsTrue()
        {
            var subjectA = (ParseResult<int>)null;
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA == subjectB, Is.True);
        }

        [Test]
        public void OpEquality_WithEqualValuesAndCursors_ReturnsTrue()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = new ParseResult<int>(start, end, 0);
            var subjectB = new ParseResult<int>(start, end, 0);

            Assert.That(subjectA == subjectB, Is.True);
        }

        [Test]
        public void OpEquality_WithNullReferenceOnLeft_ReturnsFalse()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = (ParseResult<int>)null;
            var subjectB = new ParseResult<int>(start, end, 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithNullReferenceOnRight_ReturnsFalse()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = new ParseResult<int>(start, end, 0);
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithUnequalEndCursors_ReturnsFalse()
        {
            var start = new Cursor("OK");
            var one = start.Advance(1);
            var two = one.Advance(1);
            var subjectA = new ParseResult<int>(start, one, 0);
            var subjectB = new ParseResult<int>(start, two, 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithUnequalStartCursors_ReturnsFalse()
        {
            var start = new Cursor("OK");
            var one = start.Advance(1);
            var two = one.Advance(1);
            var subjectA = new ParseResult<int>(start, two, 0);
            var subjectB = new ParseResult<int>(one, two, 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithUnequalValues_ReturnsFalse()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = new ParseResult<int>(start, end, 1);
            var subjectB = new ParseResult<int>(end, end, 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpInequality_WithBothSidesNullReference_ReturnsFalse()
        {
            var subjectA = (ParseResult<int>)null;
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA != subjectB, Is.False);
        }

        [Test]
        public void OpInequality_WithEqualValuesAndCursors_ReturnsFalse()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = new ParseResult<int>(start, end, 0);
            var subjectB = new ParseResult<int>(start, end, 0);

            Assert.That(subjectA != subjectB, Is.False);
        }

        [Test]
        public void OpInequality_WithNullReferenceOnLeft_ReturnsTrue()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = (ParseResult<int>)null;
            var subjectB = new ParseResult<int>(start, end, 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithNullReferenceOnRight_ReturnsTrue()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = new ParseResult<int>(start, end, 0);
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithUnequalEndCursors_ReturnsTrue()
        {
            var start = new Cursor("OK");
            var one = start.Advance(1);
            var two = one.Advance(1);
            var subjectA = new ParseResult<int>(start, one, 0);
            var subjectB = new ParseResult<int>(start, two, 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithUnequalStartCursors_ReturnsTrue()
        {
            var start = new Cursor("OK");
            var one = start.Advance(1);
            var two = one.Advance(1);
            var subjectA = new ParseResult<int>(start, two, 0);
            var subjectB = new ParseResult<int>(one, two, 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithUnequalValues_ReturnsTrue()
        {
            var start = new Cursor("OK");
            var end = start.Advance(1);
            var subjectA = new ParseResult<int>(start, end, 1);
            var subjectB = new ParseResult<int>(start, end, 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void GetHashCode_WithEqualValuesAndCursors_ReturnsSameValue([Values(0, 1, 2)] int index)
        {
            var start = new Cursor("OK");
            var end = start.Advance(index);
            var subjectA = new ParseResult<int>(start, end, 0);
            var subjectB = new ParseResult<int>(start, end, 0);

            Assert.That(subjectB.GetHashCode(), Is.EqualTo(subjectA.GetHashCode()));
        }
    }
}
