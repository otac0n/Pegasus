// -----------------------------------------------------------------------
// <copyright file="PegCompiler.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench.Pipeline
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Pegasus.Expressions;
    using CompileResult = Pegasus.Compiler.CompileResult;

    internal class PegCompiler : IDisposable
    {
        private readonly IDisposable disposable;

        public PegCompiler(IObservable<Grammar> grammars)
        {
            var compileResults = grammars
                .Select(r => r == null ? new CompileResult(r) : Pegasus.Compiler.PegCompiler.Compile(r))
                .Publish();

            this.Codes = compileResults.Select(r => r.Code);
            this.Errors = compileResults.Select(r => r.Errors.ToList().AsReadOnly());

            this.disposable = compileResults.Connect();
        }

        public IObservable<string> Codes { get; private set; }

        public IObservable<IList<CompilerError>> Errors { get; private set; }

        public void Dispose()
        {
            this.disposable.Dispose();
        }
    }
}
