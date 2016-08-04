// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportCodeSyntaxIssuesPass : CompilePass
    {
        public override IList<string> BlockedByErrors => new string[0];

        public override IList<string> ErrorsProduced => new[] { "CS0000", "CS1026" };

        public override void Run(Grammar grammar, CompileResult result) => new CodeSyntaxTreeWalker(result).WalkGrammar(grammar);

        private class CodeSyntaxTreeWalker : ExpressionTreeWalker
        {
            private readonly CompileResult result;

            public CodeSyntaxTreeWalker(CompileResult result)
            {
                this.result = result;
            }

            protected override void WalkCodeExpression(CodeExpression codeExpression)
            {
                var ix = codeExpression.CodeType == CodeType.State
                    ? Tuple.Create("state => {", "}")
                    : Tuple.Create("state =>", string.Empty);
                var prefix = ix.Item1;
                var suffix = ix.Item2;

                var startCursor = codeExpression.CodeSpan.Start;
                var code = (prefix + codeExpression.CodeSpan.Code + suffix).TrimEnd();
                var expression = SyntaxFactory.ParseExpression(code);

                if (expression.ContainsDiagnostics)
                {
                    foreach (var diag in expression.GetDiagnostics())
                    {
                        var cursor = diag.Location.IsInSource
                            ? startCursor.Advance(-prefix.Length + diag.Location.SourceSpan.Start)
                            : startCursor;

                        this.result.Errors.Add(new CompilerError(startCursor.FileName ?? string.Empty, cursor.Line, cursor.Column, diag.Id, diag.GetMessage()) { IsWarning = diag.WarningLevel > 0 });
                    }
                }

                if (expression.Span.Length != code.Length)
                {
                    var sliced = code.Substring(expression.Span.Length);
                    var trimmed = sliced.TrimStart();
                    this.result.AddCompilerError(startCursor.Advance(-prefix.Length + expression.Span.Length + (sliced.Length - trimmed.Length)), () => Resources.CS1026_ERROR_UnexpectedCharacter, trimmed[0]);
                }
            }
        }
    }
}
