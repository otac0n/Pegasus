// -----------------------------------------------------------------------
// <copyright file="CompileManager.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using Pegasus.Compiler;
    using Pegasus.Parser;

    internal static class CompileManager
    {
        public static void CompileFile(string inputFile, string outputFile, Action<CompilerError> logError)
        {
            outputFile = outputFile ?? inputFile + ".cs";

            var subject = File.ReadAllText(inputFile);
            var parser = new PegParser();
            var grammar = parser.Parse(subject);
            var compiler = new PegCompiler();
            var result = compiler.Compile(grammar);

            bool hadFatal = false;
            foreach (var error in result.Errors)
            {
                hadFatal |= !error.IsWarning;
                logError(error);
            }

            if (!hadFatal)
            {
                File.WriteAllText(outputFile, result.Code);
            }
        }
    }
}
