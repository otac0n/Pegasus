// -----------------------------------------------------------------------
// <copyright file="CodeGenerator.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Pegasus.Expressions;

    internal partial class CodeGenerator : ExpressionTreeWalker
    {
        private static HashSet<string> keywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte",
            "case", "catch", "char", "checked", "class", "const",
            "continue", "decimal", "default", "delegate", "do", "double",
            "else", "enum", "event", "explicit", "extern", "false",
            "finally", "fixed", "float", "for", "foreach", "goto",
            "if", "implicit", "in", "int", "interface", "internal",
            "is", "lock", "long", "namespace", "new", "null",
            "object", "operator", "out", "override", "params", "private",
            "protected", "public", "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw", "true", "try",
            "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
            "using", "virtual", "void", "volatile", "while",
        };

        private static Dictionary<char, string> simpleEscapeChars = new Dictionary<char, string>()
        {
            { '\'', "\\'" }, { '\"', "\\\"" }, { '\\', "\\\\" }, { '\0', "\\0" },
            { '\a', "\\a" }, { '\b', "\\b" }, { '\f', "\\f" }, { '\n', "\\n" },
            { '\r', "\\r" }, { '\t', "\\t" }, { '\v', "\\v" },
        };

        private readonly Dictionary<string, int> variables = new Dictionary<string, int>();
        private readonly TextWriter writer;
        private readonly Dictionary<Expression, object> types;
        private readonly HashSet<Rule> leftRecursiveRules;

        private Grammar grammar;
        private string currentIndentation;
        private string currentResultName = null;
        private object currentResultType = null;

        public CodeGenerator(TextWriter writer, Dictionary<Expression, object> types, HashSet<Rule> leftRecursiveRules)
        {
            this.writer = writer;
            this.types = types;
            this.leftRecursiveRules = leftRecursiveRules;
        }

        public override void WalkGrammar(Grammar grammar)
        {
            this.RenderGrammar(grammar, this.writer, this.currentIndentation);
        }

        protected override void WalkAndCodeExpression(AndCodeExpression andCodeExpression)
        {
            this.RenderCodeAssertion(new { Code = andCodeExpression.Code, MustMatch = true }, this.writer, this.currentIndentation);
        }

        protected override void WalkAndExpression(AndExpression andExpression)
        {
            this.RenderAssertion(new { Expression = andExpression.Expression, MustMatch = true }, this.writer, this.currentIndentation);
        }

        protected override void WalkChoiceExpression(ChoiceExpression choiceExpression)
        {
            this.RenderChoiceExpression(choiceExpression, this.writer, this.currentIndentation);
        }

        protected override void WalkClassExpression(ClassExpression classExpression)
        {
            this.RenderClassExpression(classExpression, this.writer, this.currentIndentation);
        }

        protected override void WalkCodeExpression(CodeExpression codeExpression)
        {
            if (codeExpression.CodeType != CodeType.State)
            {
                throw new InvalidOperationException("Code expressions are only valid at the end of a sequence expression.");
            }

            this.RenderCodeExpression(codeExpression, this.writer, this.currentIndentation);
        }

        protected override void WalkLiteralExpression(LiteralExpression literalExpression)
        {
            this.RenderLiteralExpression(literalExpression, this.writer, this.currentIndentation);
        }

        protected override void WalkNameExpression(NameExpression nameExpression)
        {
            this.RenderNameExpression(nameExpression, this.writer, this.currentIndentation);
        }

        protected override void WalkNotCodeExpression(NotCodeExpression notCodeExpression)
        {
            this.RenderCodeAssertion(new { Code = notCodeExpression.Code, MustMatch = false }, this.writer, this.currentIndentation);
        }

        protected override void WalkNotExpression(NotExpression notExpression)
        {
            this.RenderAssertion(new { Expression = notExpression.Expression, MustMatch = false }, this.writer, this.currentIndentation);
        }

        protected override void WalkPrefixedExpression(PrefixedExpression prefixedExpression)
        {
            this.RenderPrefixedExpression(prefixedExpression, this.writer, this.currentIndentation);
        }

        protected override void WalkRepetitionExpression(RepetitionExpression repetitionExpression)
        {
            this.RenderRepetitionExpression(repetitionExpression, this.writer, this.currentIndentation);
        }

        protected override void WalkRule(Rule rule)
        {
            this.RenderRule(rule, this.writer, this.currentIndentation);
        }

        protected override void WalkSequenceExpression(SequenceExpression sequenceExpression)
        {
            this.RenderSequenceExpression(sequenceExpression, this.writer, this.currentIndentation);
        }

        protected override void WalkWildcardExpression(WildcardExpression wildcardExpression)
        {
            this.RenderWildcardExpression(wildcardExpression, this.writer, this.currentIndentation);
        }

        private static string EscapeName(object name)
        {
            if (name == null)
            {
                return null;
            }

            var n = name.ToString();
            return keywords.Contains(n) ? "@" + n : n;
        }

        private static string ToLiteral(string input)
        {
            var sb = new StringBuilder(input.Length * 2);
            sb.Append("\"");
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];

                string literal;
                if (simpleEscapeChars.TryGetValue(c, out literal))
                {
                    sb.Append(literal);
                }
                else if (c >= 32 && c <= 126)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                }
            }

            sb.Append("\"");
            return sb.ToString();
        }

        private string CreateVariable(string prefix)
        {
            int instance;
            this.variables.TryGetValue(prefix, out instance);
            this.variables[prefix] = instance + 1;
            return prefix + instance;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "writer", Justification = "Required by Weave.")]
        private void WalkExpression(Expression expression, TextWriter writer, string indentation)
        {
            var temp = this.currentIndentation;
            this.currentIndentation = indentation;
            this.WalkExpression(expression);
            this.currentIndentation = temp;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "writer", Justification = "Required by Weave.")]
        private void WalkGrammar(Grammar grammar, TextWriter writer, string indentation)
        {
            var temp = this.currentIndentation;
            this.currentIndentation = indentation;
            base.WalkGrammar(grammar);
            this.currentIndentation = temp;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "writer", Justification = "Required by Weave.")]
        private void WalkRule(Rule rule, TextWriter writer, string indentation)
        {
            var temp = this.currentIndentation;
            this.currentIndentation = indentation;
            base.WalkRule(rule);
            this.currentIndentation = temp;
        }
    }
}
