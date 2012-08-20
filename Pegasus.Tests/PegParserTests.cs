// -----------------------------------------------------------------------
// <copyright file="PegParserTests.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using System.IO;
    using NUnit.Framework;
    using Pegasus.Parser;

    public class PegParserTests
    {
        [Test]
        public void Parse_WithPegGrammar_Works()
        {
            var subject = File.ReadAllText("peg.peg");
            var parser = new PegParser();

            var result = parser.Parse(subject);
            Assert.That(result, Is.Not.Null);
        }
    }
}
