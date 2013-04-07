// -----------------------------------------------------------------------
// <copyright file="GenerateCodePass.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Pegasus.Expressions;

    internal class GenerateCodePass : CompilePass
    {
        public override IList<string> ErrorsProduced
        {
            get { return new string[0]; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new[] { "PEG0001", "PEG0002", "PEG0003", "PEG0004", "PEG0005", "PEG0007", "PEG0012", "PEG0016" }; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                new CodeGenerator(stringWriter).WalkGrammar(grammar);
                result.Code = stringWriter.ToString();
            }
        }
    }
}
