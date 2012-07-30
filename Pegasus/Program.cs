// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System.IO;
    using Pegasus.Compiler;

    internal class Program
    {
        public static void Main(string[] args)
        {
            var input = File.ReadAllText(args[0]);
            var parser = new PegParser();
            var grammar = parser.Parse(input);
            var compiler = new PegCompiler();
            var result = compiler.Compile(grammar.Value);
            File.WriteAllText(Path.GetFileNameWithoutExtension(args[0]) + ".cs", result.Code);
        }
    }
}
