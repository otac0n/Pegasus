// -----------------------------------------------------------------------
// <copyright file="CompilePass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using Pegasus.Expressions;

    internal abstract class CompilePass
    {
        public abstract IList<string> ErrorsProduced { get; }

        public abstract IList<string> BlockedByErrors { get; }

        public abstract void Run(Grammar grammar, CompileResult result);
    }
}
