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
    using System.Text.RegularExpressions;
    using Pegasus.Common;
    using Pegasus.Compiler;
    using Pegasus.Expressions;
    using Pegasus.Parser;

    internal static class CompileManager
    {
        public static void CompileFile(string inputFile, string outputFile, Action<CompilerError> logError)
        {
            outputFile = outputFile ?? inputFile + ".cs";

            var subject = File.ReadAllText(inputFile);
            var parser = new PegParser();
            Grammar grammar;
            try
            {
                grammar = parser.Parse(subject, fileName: inputFile);
            }
            catch (FormatException ex)
            {
                var cursor = ex.Data["cursor"] as Cursor;
                if (cursor != null && Regex.IsMatch(ex.Message, @"^PEG\d+:"))
                {
                    var parts = ex.Message.Split(new[] { ':' }, 2);
                    logError(new CompilerError(cursor.FileName, cursor.Line, cursor.Column, parts[0], parts[1]));
                    return;
                }

                throw;
            }

            var result = PegCompiler.Compile(grammar);

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
