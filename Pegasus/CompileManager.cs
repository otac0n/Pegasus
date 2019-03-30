// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

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

            outputFile = outputFile ?? inputFile + ".g.cs";

            var subject = File.ReadAllText(inputFile);
            var result = CompileString(subject, fileName: MakePragmaPath(inputFile, outputFile));

            var hadFatal = false;
            foreach (var error in result.Errors)
            {
                hadFatal |= !error.IsWarning;
                logError(error);
            }

            if (!hadFatal)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                File.WriteAllText(outputFile, result.Code);
            }
        }

        /// <summary>
        /// Parse and compile a PEG grammar from a string.
        /// </summary>
        /// <param name="subject">The PEG grammar to parse and compile.</param>
        /// <param name="fileName">The filename to use in errors.</param>
        /// <returns>A <see cref="CompileResult"/> containing the result of the compilation.</returns>
        public static CompileResult CompileString(string subject, string fileName)
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
                    result.Errors.Add(new CompilerError(cursor.FileName ?? string.Empty, cursor.Line, cursor.Column, parts[0], parts[1]));
                    return result;
                }

                throw;
            }

            return PegCompiler.Compile(grammar);
        }

        /// <summary>
        /// Compares the input and output path and returns the appropriate filename to use in <c>#line</c> pragmas.
        /// </summary>
        /// <param name="input">The input file path.</param>
        /// <param name="output">The output file path.</param>
        /// <returns>The input path transformed to the appropriate pragma path.</returns>
        public static string MakePragmaPath(string input, string output)
        {
            output = Path.GetFullPath(output);
            input = Path.GetFullPath(input);
            var relativeUri = new Uri(output).MakeRelativeUri(new Uri(input));
            return relativeUri.ToString().IndexOf('/') != -1
                ? input
                : Path.GetFileName(input);
        }
    }
}
