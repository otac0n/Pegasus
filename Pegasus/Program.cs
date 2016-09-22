// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using Pegasus.Properties;

    internal class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return -1;
            }

            var errorCount = 0;
            foreach (var arg in args)
            {
                var errors = new List<CompilerError>();
                CompileManager.CompileFile(arg, null, errors.Add);
                ShowErrors(errors);
                errorCount += errors.Count;
            }

            return errorCount;
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
            Console.WriteLine(Resources.Usage);
        }
    }
}
