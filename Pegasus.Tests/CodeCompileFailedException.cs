// -----------------------------------------------------------------------
// <copyright file="CodeCompileFailedException.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;

    public class CodeCompileFailedException : Exception
    {
        public CodeCompileFailedException(IEnumerable<CompilerError> errors, IEnumerable<string> messages)
        {
            this.Errors = errors.ToList().AsReadOnly();
            this.Messages = messages.ToList().AsReadOnly();
        }

        public IList<CompilerError> Errors { get; private set; }

        public IList<string> Messages { get; private set; }
    }
}
