// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    internal class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                ShowUsage();
                return -1;
            }

            var errors = new List<CompilerError>();
            CompileManager.CompileFile(args[0], null, errors.Add);
            ShowErrors(errors);

            return errors.Count;
        }

        private static void ShowErrors(List<CompilerError> errors)
        {
            var startingColor = Console.ForegroundColor;

            foreach (var e in errors)
            {
                Console.ForegroundColor = e.IsWarning ? ConsoleColor.Yellow : ConsoleColor.Red;
                Console.WriteLine(e);
            }

            Console.ForegroundColor = startingColor;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    pegasus file");
        }
    }
}
