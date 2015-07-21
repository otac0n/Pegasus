// -----------------------------------------------------------------------
// <copyright file="GenerateCodePass.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class GenerateCodePass : CompilePass
    {
        private static readonly Lazy<IList<string>> AllErrors = new Lazy<IList<string>>(() =>
        {
            var additionalErrors = new[] { "CS0000" };

            return (from p in typeof(Resources).GetProperties(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty)
                    let parts = p.Name.Split('_')
                    where parts.Length == 3
                    where parts[1] == "ERROR"
                    select parts[0])
                    .Concat(additionalErrors)
                    .ToList()
                    .AsReadOnly();
        });

        public override IList<string> BlockedByErrors
        {
            get { return AllErrors.Value; }
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
