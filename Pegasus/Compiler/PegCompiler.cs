// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Pegasus.Expressions;

    /// <summary>
    /// Provides error checking and compilation services for PEG grammars.
    /// </summary>
    public static class PegCompiler
    {
        private static readonly IList<Type> PassTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(CompilePass)))
            .ToList()
            .AsReadOnly();

        /// <summary>
        /// Compiles a PEG grammar into a program.
        /// </summary>
        /// <param name="grammar">The grammar to compile.</param>
        /// <returns>A <see cref="CompileResult"/> containing the errors, warnings, and results of compilation.</returns>
        public static CompileResult Compile(Grammar grammar)
        {
            var result = new CompileResult(grammar);

            var passes = PassTypes.Select(t => (CompilePass)Activator.CreateInstance(t)).ToList();
            while (true)
            {
                var existingErrors = new HashSet<string>(result.Errors.Where(e => !e.IsWarning).Select(e => e.ErrorNumber));
                var pendingErrors = new HashSet<string>(passes.SelectMany(p => p.ErrorsProduced));

                var nextPasses = passes
                    .Where(p => !p.BlockedByErrors.Any(e => existingErrors.Contains(e)))
                    .Where(p => !p.BlockedByErrors.Any(e => pendingErrors.Contains(e)))
                    .ToList();

                if (nextPasses.Count == 0)
                {
                    break;
                }

                foreach (var pass in nextPasses)
                {
                    pass.Run(grammar, result);
                    passes.Remove(pass);
                }
            }

            return result;
        }
    }
}
