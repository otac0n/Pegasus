// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System;
    using System.IO;
    using System.Linq;
    using Pegasus.Compiler;
    using Pegasus.Parser;

    internal class Program
    {
        public static void Main(string[] args)
        {
            var input = File.ReadAllText(args[0]);
            var parser = new PegParser();
            var grammar = parser.Parse(input);
            var compiler = new PegCompiler();
            var result = compiler.Compile(grammar);
            foreach (var error in result.Errors)
            {
                Console.Error.WriteLine(error.ToString());
            }

            if (!result.Errors.Any(e => !e.IsWarning))
            {
                File.WriteAllText(args[0] + ".cs", result.Code);
            }
        }
    }
}
