// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

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

    /// <summary>
    /// Provides a simple interface for parsing and compiling a PEG grammar.
    /// </summary>
    public static class CompileManager
    {
        /// <summary>
        /// Parse and compile a PEG grammar from a file.
        /// </summary>
        /// <param name="inputFile">The source filename.</param>
        /// <param name="outputFile">The desired destination filename, or <c>null</c> to use the default.</param>
        /// <param name="logError">An action that will be called for every warning or error.</param>
        public static void CompileFile(string inputFile, string outputFile, Action<CompilerError> logError)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentNullException(nameof(inputFile));
            }

            if (logError == null)
            {
                throw new ArgumentNullException(nameof(logError));
            }

            outputFile = outputFile ?? inputFile + ".cs";

            var subject = File.ReadAllText(inputFile);
            var result = CompileString(subject, fileName: inputFile);

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

        /// <summary>
        /// Parse and compile a PEG grammar from a string.
        /// </summary>
        /// <param name="subject">The PEG grammar to parse and compile.</param>
        /// <param name="fileName">The filename to use in errors.</param>
        /// <returns>A <see cref="CompileResult"/> containing the result of the compilation.</returns>
        public static CompileResult CompileString(string subject, string fileName = null)
        {
            Grammar grammar;
            try
            {
                grammar = new PegParser().Parse(subject ?? string.Empty, fileName);
            }
            catch (FormatException ex)
            {
                var cursor = ex.Data["cursor"] as Cursor;
                if (cursor != null && Regex.IsMatch(ex.Message, @"^PEG\d+:"))
                {
                    var parts = ex.Message.Split(new[] { ':' }, 2);
                    var result = new CompileResult(null);
                    result.Errors.Add(new CompilerError(cursor.FileName, cursor.Line, cursor.Column, parts[0], parts[1]));
                    return result;
                }

                throw;
            }

            return PegCompiler.Compile(grammar);
        }
    }
}
