// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Common
{
    using NUnit.Framework;
    using Pegasus.Common;

    [TestFixture]
    public class CacheKeyTests
    {
        [Test]
        public void Equals_WithDifferentCacheKeys_ReturnsFalse()
        {
            var subjectA = new CacheKey("OK", 0, 0);
            var subjectB = new CacheKey("OK", 1, 0);

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }

        [Test]
        public void Equals_WithIdenticalCacheKeys_ReturnsTrue([Values(0, 1, 2)] int stateKey, [Values(0, 1, 2)] int location)
        {
            var subjectA = new CacheKey("OK", stateKey, location);
            var subjectB = new CacheKey("OK", stateKey, location);

            Assert.That(subjectA.Equals(subjectB), Is.True);
        }

        [Test]
        public void Equals_WithNullReference_ReturnsFalse()
        {
            var subjectA = new CacheKey("OK", 0, 0);
            var subjectB = (CacheKey)null;

            Assert.That(subjectA.Equals(subjectB), Is.False);
        }

        [Test]
        public void GetHashCode_WithIdenticalCacheKeys_ReturnsTheSameValue([Values(0, 1, 2)] int stateKey, [Values(0, 1, 2)] int location)
        {
            var subjectA = new CacheKey("OK", stateKey, location);
            var subjectB = new CacheKey("OK", stateKey, location);

            Assert.That(subjectA.GetHashCode(), Is.EqualTo(subjectB.GetHashCode()));
        }
    }
}
