// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CSharp;
    using Pegasus.Common;

    public static class CodeCompiler
    {
        public static ParserWrapper<T> Compile<T>(string source)
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

            return new ParserWrapper<T>(type);
        }

        public class ParserWrapper<T>
        {
            private readonly object instance;
            private readonly Type type;

            public ParserWrapper(Type type)
            {
                this.type = type;
                this.instance = Activator.CreateInstance(type);
            }

            public T Parse(string subject, string fileName = null)
            {
                return (T)this.instance
                    .GetType()
                    .GetMethod("Parse", new[] { typeof(string), typeof(string) })
                    .Invoke(this.instance, new[] { subject, fileName });
            }

            public T Parse(string subject, string fileName, out IList<LexicalElement> lexicalElements)
            {
                var args = new object[] { subject, fileName, null };
                var result = (T)this.instance
                    .GetType()
                    .GetMethod("Parse", new[] { typeof(string), typeof(string), typeof(IList<LexicalElement>).MakeByRefType() })
                    .Invoke(this.instance, args);
                lexicalElements = (IList<LexicalElement>)args[2];
                return result;
            }
        }
    }
}
