// -----------------------------------------------------------------------
// <copyright file="GenerateCodePass.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Pegasus.Expressions;

    internal class GenerateCodePass : CompilePass
    {
        public override IList<string> BlockedByErrors
        {
            get { return new[] { "CS0000", "PEG0001", "PEG0002", "PEG0003", "PEG0005", "PEG0007", "PEG0008", "PEG0012", "PEG0016", "PEG0019", "PEG0020", "PEG0021" }; }
        }

        public override IList<string> ErrorsProduced
        {
            get { return new string[0]; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            if (result.Errors.Any(e => !e.IsWarning))
            {
                return;
            }

            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                new CodeGenerator(stringWriter, result.ExpressionTypes, result.LeftRecursiveRules).WalkGrammar(grammar);
                result.Code = stringWriter.ToString();
            }
        }
    }
}
