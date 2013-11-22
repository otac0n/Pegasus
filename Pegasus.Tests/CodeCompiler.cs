// -----------------------------------------------------------------------
// <copyright file="CodeCompiler.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Linq;
    using Microsoft.CSharp;

    public static class CodeCompiler
    {
        public static Func<string, string, T> Compile<T>(string source)
        {
            var compiler = new CSharpCodeProvider();
            var options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add(typeof(Pegasus.Common.Cursor).Assembly.Location);

            var results = compiler.CompileAssemblyFromSource(options, source);
            if (results.Errors.HasErrors)
            {
                throw new CodeCompileFailedException(results.Errors.Cast<CompilerError>().ToArray(), results.Output.Cast<string>().ToArray());
            }

            var assembly = results.CompiledAssembly;
            var type = assembly.GetType("Parsers.Parser");
            var method = type.GetMethod("Parse");

            var @this = Activator.CreateInstance(type);
            return (Func<string, string, T>)Delegate.CreateDelegate(typeof(Func<string, string, T>), @this, method);
        }
    }
}
