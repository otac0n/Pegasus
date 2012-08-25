// -----------------------------------------------------------------------
// <copyright file="ParseResultTests.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using NUnit.Framework;
    using Pegasus.Parser;

    public class ParseResultTests
    {
        [Test]
        public void OpEquality_WithBothSidesNullReference_ReturnsTrue()
        {
            var subjectA = (ParseResult<int>)null;
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA == subjectB, Is.True);
        }

        [Test]
        public void OpEquality_WithNullReferenceOnRight_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithNullReferenceOnLeft_ReturnsFalse()
        {
            var subjectA = (ParseResult<int>)null;
            var subjectB = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithEqualValuesAndCursors_ReturnsTrue()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);
            var subjectB = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);

            Assert.That(subjectA == subjectB, Is.True);
        }

        [Test]
        public void OpEquality_WithUnequalValues_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 1);
            var subjectB = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithUnequalStartCursors_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 2), 0);
            var subjectB = new ParseResult<int>(new Cursor("OK", 1), new Cursor("OK", 2), 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithUnequalEndCursors_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);
            var subjectB = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 2), 0);

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
        public void OpInequality_WithNullReferenceOnRight_ReturnsTrue()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithNullReferenceOnLeft_ReturnsTrue()
        {
            var subjectA = (ParseResult<int>)null;
            var subjectB = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithEqualValuesAndCursors_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);
            var subjectB = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);

            Assert.That(subjectA != subjectB, Is.False);
        }

        [Test]
        public void OpInequality_WithUnequalValues_ReturnsTrue()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 1);
            var subjectB = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithUnequalStartCursors_ReturnsTrue()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 2), 0);
            var subjectB = new ParseResult<int>(new Cursor("OK", 1), new Cursor("OK", 2), 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithUnequalEndCursors_ReturnsTrue()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);
            var subjectB = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 2), 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void Equals_WithOtherObject_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(new Cursor("OK", 0), new Cursor("OK", 1), 0);
            var subjectB = new object();

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }
    }
}
