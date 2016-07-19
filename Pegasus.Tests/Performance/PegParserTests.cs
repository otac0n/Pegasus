// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Performance
{
    using System.IO;
    using NUnit.Framework;
    using Pegasus.Parser;
    using static PerformanceTests;

    [TestFixture]
    [Category("Performance")]
    public class PegParserTests
    {
        private readonly string emptyGrammar;
        private readonly string pegGrammar;

        public PegParserTests()
        {
            this.emptyGrammar = "a = ((())) ((() (() (((()) (() () ()))) ()) () (((((() (( ()) () (( ())))) (() (()))) () () (())) () (() (() () (())) ()) () () (((( ()) ())) ((()) (()) ) () () (((()) ) ()) ) ((() (() ()) ((()) ())))) ) ()) (((() ((() ((()) ()) ())) (() (() (()) () ((())))))) (((() ()))) (() ((() (()) () (()) (() ()))) ()) ()) () ()) ()";
            this.pegGrammar = File.ReadAllText("PegParser.peg");
        }

        [Test]
        public void EmptyGrammar()
        {
            Evaluate(() =>
            {
                new PegParser().Parse(this.emptyGrammar);
            });
        }

        [Test]
        public void PegGrammar()
        {
            Evaluate(() =>
            {
                new PegParser().Parse(this.pegGrammar);
            });
        }
    }
}
