// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Expressions
{
    using System;
    using NUnit.Framework;
    using Pegasus.Expressions;

    [TestFixture]
    public class CodeExpressionTests
    {
        [Theory]
        public void Constructor_WhenGivenANullCodeSpan_ThrowsException(CodeType codeType)
        {
            Assert.That(() => new CodeExpression(null, codeType), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
