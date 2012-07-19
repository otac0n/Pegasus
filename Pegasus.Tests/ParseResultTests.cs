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
            var subjectA = new ParseResult<int>(1, 0);
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithNullReferenceOnLeft_ReturnsFalse()
        {
            var subjectA = (ParseResult<int>)null;
            var subjectB = new ParseResult<int>(1, 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithEqualValuesAndLengths_ReturnsTrue()
        {
            var subjectA = new ParseResult<int>(1, 0);
            var subjectB = new ParseResult<int>(1, 0);

            Assert.That(subjectA == subjectB, Is.True);
        }

        [Test]
        public void OpEquality_WithUnequalValues_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(1, 1);
            var subjectB = new ParseResult<int>(1, 0);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithUnequalLengths_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(1, 0);
            var subjectB = new ParseResult<int>(2, 0);

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
            var subjectA = new ParseResult<int>(1, 0);
            var subjectB = (ParseResult<int>)null;

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithNullReferenceOnLeft_ReturnsTrue()
        {
            var subjectA = (ParseResult<int>)null;
            var subjectB = new ParseResult<int>(1, 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithEqualValuesAndLengths_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(1, 0);
            var subjectB = new ParseResult<int>(1, 0);

            Assert.That(subjectA != subjectB, Is.False);
        }

        [Test]
        public void OpInequality_WithUnequalValues_ReturnsTrue()
        {
            var subjectA = new ParseResult<int>(1, 1);
            var subjectB = new ParseResult<int>(1, 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithUnequalLengths_ReturnsTrue()
        {
            var subjectA = new ParseResult<int>(1, 0);
            var subjectB = new ParseResult<int>(2, 0);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void Equals_WithOtherObject_ReturnsFalse()
        {
            var subjectA = new ParseResult<int>(1, 0);
            var subjectB = new object();

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }
    }
}
