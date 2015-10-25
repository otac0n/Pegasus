// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;

    public class CodeCompileFailedException : Exception
    {
        public CodeCompileFailedException(IEnumerable<CompilerError> errors, IEnumerable<string> messages)
            : base("Compile failed:" + Environment.NewLine + string.Join(Environment.NewLine, errors.Select(e => e.ToString())))
        {
            this.Errors = errors.ToList().AsReadOnly();
            this.Messages = messages.ToList().AsReadOnly();
        }

        public IList<CompilerError> Errors { get; }

        public IList<string> Messages { get; }
    }
}
