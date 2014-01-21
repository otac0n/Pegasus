// -----------------------------------------------------------------------
// <copyright file="CursorTests.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using NUnit.Framework;
    using Pegasus.Common;

    [TestFixture]
    public class CursorTests
    {
        [Test]
        public void Constructor_WhenGivenALocationPastTheEndOfTheString_ThrowsException()
        {
            Assert.That(() => new Cursor("", 1), Throws.Exception);
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
        public void WithMutability_WhenTheStateHasBeenModified_ReturnsAnUnequalCursor()
        {
            var cursor = new Cursor("OK", 0);
            var mutable = cursor.WithMutability(true);
            mutable["OK"] = "OK";

            var result = mutable.WithMutability(false);

            Assert.That(result, Is.Not.EqualTo(cursor));
        }

        [Test]
        public void WithMutability_WhenTheStateHasNotBeenModified_ReturnsAnEqualCursor()
        {
            var cursor = new Cursor("OK", 0);
            var mutable = cursor.WithMutability(true);

            var result = mutable.WithMutability(false);

            Assert.That(result, Is.EqualTo(cursor));
        }
    }
}
