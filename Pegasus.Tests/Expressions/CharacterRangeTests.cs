// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using NUnit.Framework;
    using Pegasus.Expressions;

    [TestFixture]
    public class CharacterRangeTests
    {
        [Datapoints]
        public readonly char[] Datapoints = { char.MinValue, 'a', 'z', 'A', 'Z', char.MaxValue };

        [Test]
        public void Constructor_WhenGivenAMinCharacterGreaterThanTheMaxCharacter_DoesNotThrow()
        {
            Assert.That(() => new CharacterRange(char.MaxValue, char.MinValue), Throws.Nothing);
        }

        [Theory]
        public void Equals_WhenConstructedWithTheSameCharacters_ReturnsTrue(char min, char max)
        {
            var subjectA = new CharacterRange(min, max);
            var subjectB = new CharacterRange(min, max);

            Assert.That(subjectA.Equals(subjectB), Is.True);
        }

        [Theory]
        public void Equals_WithDifferentMaxCharacter_ReturnsFalse(char min, char maxA, char maxB)
        {
            Assume.That(maxA, Is.Not.EqualTo(maxB));

            var subjectA = new CharacterRange(min, maxA);
            var subjectB = new CharacterRange(min, maxB);

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }

        [Theory]
        public void Equals_WithDifferentMinCharacter_ReturnsFalse(char minA, char minB, char max)
        {
            Assume.That(minA, Is.Not.EqualTo(minB));

            var subjectA = new CharacterRange(minA, max);
            var subjectB = new CharacterRange(minB, max);

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }

        [Test]
        public void Equals_WithNullReference_ReturnsFalse()
        {
            var subjectA = new CharacterRange(char.MinValue, char.MaxValue);
            var subjectB = (object)null;

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }

        [Test]
        public void Equals_WithOtherObject_ReturnsFalse()
        {
            var subjectA = new CharacterRange(char.MinValue, char.MaxValue);
            var subjectB = new object();

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }

        [Theory]
        public void GetHashCode_WhenConstructedWithTheSameCharacters_ReturnsTheSameValue(char min, char max)
        {
            var subjectA = new CharacterRange(min, max);
            var subjectB = new CharacterRange(min, max);

            Assert.That(subjectA.GetHashCode(), Is.EqualTo(subjectB.GetHashCode()));
        }
    }
}
