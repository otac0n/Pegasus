// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Common
{
    using System;
    using NUnit.Framework;
    using Pegasus.Common;

    [TestFixture]
    public class CursorTests
    {
        [Test]
        public void Advance_WhenTheCursorIsMutable_ThrowsException()
        {
            var cursor = new Cursor("OK", 0).WithMutability(mutable: true);
            Assert.That(() => cursor.Advance(0), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void Constructor_WhenGivenALocationLessThanZero_ThrowsException()
        {
            Assert.That(() => new Cursor(string.Empty, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_WhenGivenALocationPastTheEndOfTheString_ThrowsException()
        {
            Assert.That(() => new Cursor(string.Empty, 1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_WhenGivenANullSubject_ThrowsException()
        {
            Assert.That(() => new Cursor(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        [TestCase("abc\ndef", 0, 1, 1)]
        [TestCase("abc\ndef", 1, 1, 2)]
        [TestCase("abc\ndef", 2, 1, 3)]
        [TestCase("abc\ndef", 3, 1, 4)]
        [TestCase("abc\ndef", 4, 2, 1)]
        [TestCase("abc\ndef", 5, 2, 2)]
        [TestCase("abc\ndef", 6, 2, 3)]
        [TestCase("abc\ndef", 7, 2, 4)]
        [TestCase("abc\r\ndef", 4, 1, 5)]
        [TestCase("abc\rdef", 4, 2, 1)]
        [TestCase("abc\u2028def", 4, 2, 1)]
        [TestCase("abc\u2029def", 4, 2, 1)]
        public void Constructor_WhenGivenASpecificLocation_SetsTheCorrectLineAndColumn(string subject, int location, int line, int column)
        {
            var cursor = new Cursor(subject, location);

            Assert.That(cursor.Line, Is.EqualTo(line));
            Assert.That(cursor.Column, Is.EqualTo(column));
        }

        [Test]
        public void GetHashCode_WithEqualSubjectsAndIndexesAndStateKey_ReturnsSameValue([Values(0, 1, 2)] int index)
        {
            var subjectA = new Cursor("OK", index);
            var subjectB = subjectA.Advance(0);

            Assert.That(subjectB.GetHashCode(), Is.EqualTo(subjectA.GetHashCode()));
        }

        [TestCase(0, 1, 1)]
        [TestCase(1, 1, 2)]
        [TestCase(2, 1, 3)]
        [TestCase(3, 1, 4)]
        [TestCase(4, 2, 1)]
        [TestCase(8, 3, 1)]
        [TestCase(12, 4, 1)]
        [TestCase(16, 5, 1)]
        [TestCase(20, 5, 5)]
        [TestCase(21, 6, 1)]
        [TestCase(25, 6, 5)]
        [TestCase(26, 7, 1)]
        public void GetLineAndColumn_Always_ReturnsExpectedValues(int index, int line, int column)
        {
            var cursor = new Cursor("OK1\rOK2\nOK3\u2028OK4\u2029OK5\r\nOK6\n\rOK7", index);

            Assert.That($"({cursor.Line},{cursor.Column})", Is.EqualTo($"({line},{column})"));
        }

        [Test]
        public void OpEquality_WithBothSidesNullReference_ReturnsTrue()
        {
            var subjectA = (Cursor)null;
            var subjectB = (Cursor)null;

            Assert.That(subjectA == subjectB, Is.True);
        }

        [Test]
        public void OpEquality_WithEqualSubjectAndIndex_ReturnsTrue()
        {
            var subjectA = new Cursor("OK");
            var subjectB = subjectA.Advance(0);

            Assert.That(subjectA == subjectB, Is.True);
        }

        [Test]
        public void OpEquality_WithNullReferenceOnLeft_ReturnsFalse()
        {
            var subjectA = (Cursor)null;
            var subjectB = new Cursor("OK");

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithNullReferenceOnRight_ReturnsFalse()
        {
            var subjectA = new Cursor("OK");
            var subjectB = (Cursor)null;

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithUnequalEndSubjects_ReturnsFalse()
        {
            var subjectA = new Cursor("OK1");
            var subjectB = new Cursor("OK2");

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpEquality_WithUnequalIndexes_ReturnsFalse()
        {
            var subjectA = new Cursor("OK");
            var subjectB = new Cursor("OK", 1);

            Assert.That(subjectA == subjectB, Is.False);
        }

        [Test]
        public void OpInequality_WithBothSidesNullReference_ReturnsFalse()
        {
            var subjectA = (Cursor)null;
            var subjectB = (Cursor)null;

            Assert.That(subjectA != subjectB, Is.False);
        }

        [Test]
        public void OpInequality_WithEqualSubjectAndIndex_ReturnsFalse()
        {
            var subjectA = new Cursor("OK");
            var subjectB = subjectA.Advance(0);

            Assert.That(subjectA != subjectB, Is.False);
        }

        [Test]
        public void OpInequality_WithNullReferenceOnLeft_ReturnsTrue()
        {
            var subjectA = (Cursor)null;
            var subjectB = new Cursor("OK");

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithNullReferenceOnRight_ReturnsTrue()
        {
            var subjectA = new Cursor("OK");
            var subjectB = (Cursor)null;

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithUnequalEndSubjects_ReturnsTrue()
        {
            var subjectA = new Cursor("OK1");
            var subjectB = new Cursor("OK2");

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void OpInequality_WithUnequalIndexes_ReturnsTrue()
        {
            var subjectA = new Cursor("OK");
            var subjectB = new Cursor("OK", 1);

            Assert.That(subjectA != subjectB, Is.True);
        }

        [Test]
        public void SetItem_WhenTheCursorIsNotMutable_ThrowsException()
        {
            var cursor = new Cursor("OK");
            Assert.That(() => cursor["OK"] = "OK", Throws.InvalidOperationException);
        }

        [TestCase("OK", 0)]
        [TestCase("OK", 1)]
        [TestCase("OK\r\nOK", 5)]
        public void Touch_Always_CreatesACursorWithIdenticalProperties(string subject, int location)
        {
            var cursor = new Cursor(subject, location);
            var touched = cursor.Touch();
            Assert.That(cursor.Location == touched.Location && cursor.Subject == touched.Subject, Is.True);
        }

        [Test]
        public void Touch_Always_CreatesADistinctCursor()
        {
            var cursor = new Cursor("OK", 0);
            var touched = cursor.Touch();
            Assert.That(cursor.Equals(touched), Is.False);
        }

        [Test]
        public void WithMutability_WhenTheStateHasBeenModified_ReturnsAnUnequalCursor()
        {
            var cursor = new Cursor("OK");
            var mutable = cursor.WithMutability(true);
            mutable["OK"] = "OK";

            var result = mutable.WithMutability(false);

            Assert.That(result, Is.Not.EqualTo(cursor));
        }

        [Test]
        public void WithMutability_WhenTheStateHasNotBeenModified_ReturnsAnEqualCursor()
        {
            var cursor = new Cursor("OK");
            var mutable = cursor.WithMutability(true);

            var result = mutable.WithMutability(false);

            Assert.That(result, Is.EqualTo(cursor));
        }
    }
}
