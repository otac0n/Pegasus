// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CSharp;
    using NUnit.Framework;
    using Pegasus.Common;
    using Pegasus.Common.Tracing;
    using Pegasus.Compiler;

    public static class CodeCompiler
    {
        public static ParserWrapper<T> Compile<T>(CompileResult result, params string[] referenceAssemblies)
        {
            Assert.That(result.Errors.Where(e => !e.IsWarning), Is.Empty);

            var compiler = new CSharpCodeProvider();
            var options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add(typeof(Pegasus.Common.Cursor).Assembly.Location);
            options.ReferencedAssemblies.AddRange(referenceAssemblies);

            var results = compiler.CompileAssemblyFromSource(options, result.Code);
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
            private readonly Lazy<PropertyInfo> tracerProperty;
            private readonly Lazy<MethodInfo> parseMethod;
            private readonly Lazy<MethodInfo> parseLexicalMethod;

            public ParserWrapper(Type type)
            {
                this.Type = type;
                this.instance = Activator.CreateInstance(type);
                this.tracerProperty = new Lazy<PropertyInfo>(() => type.GetProperty("Tracer", typeof(ITracer)));
                this.parseMethod = new Lazy<MethodInfo>(() => type.GetMethod("Parse", new[] { typeof(string), typeof(string) }));
                this.parseLexicalMethod = new Lazy<MethodInfo>(() => type.GetMethod("Parse", new[] { typeof(string), typeof(string), typeof(IList<LexicalElement>).MakeByRefType() }));
            }

            public Type Type { get; }

            public ITracer Tracer
            {
                get
                {
                    try
                    {
                        return (ITracer)this.tracerProperty.Value.GetValue(this.instance);
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                }

                set
                {
                    try
                    {
                        this.tracerProperty.Value.SetValue(this.instance, value);
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }

            public T Parse(string subject, string fileName = null)
            {
                try
                {
                    return (T)this.parseMethod.Value.Invoke(this.instance, new[] { subject, fileName });
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }

            public T Parse(string subject, string fileName, out IList<LexicalElement> lexicalElements)
            {
                try
                {
                    var args = new object[] { subject, fileName, null };
                    var result = (T)this.parseLexicalMethod.Value.Invoke(this.instance, args);
                    lexicalElements = (IList<LexicalElement>)args[2];
                    return result;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
        }
    }
}
