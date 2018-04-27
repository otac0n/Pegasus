// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Provides result type finding services for Pegasus <see cref="Grammar">Grammars</see>.
    /// </summary>
    public static class ResultTypeFinder
    {
        /// <summary>
        /// Finds the known result types of all expressions in the <see cref="Grammar"/>.
        /// </summary>
        /// <param name="grammar">The <see cref="Grammar"/> to inspect.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of expressions and their types.</returns>
        public static Dictionary<Expression, object> Find(Grammar grammar)
        {
            var types = new Dictionary<Expression, object>();
            var walker = new ResultTypeTreeWalker(grammar, types);

            do
            {
                walker.WalkGrammar(grammar);
            }
            while (walker.Changed);

            return types;
        }

        private class ResultTypeTreeWalker : ExpressionTreeWalker
        {
            private readonly Dictionary<string, Rule> rules;
            private bool changed;
            private Dictionary<Expression, object> types;

            public ResultTypeTreeWalker(Grammar grammar, Dictionary<Expression, object> types)
            {
                this.rules = grammar.Rules.ToDictionary(r => r.Identifier.Name);
                this.types = types;
            }

            public bool Changed => this.changed;

            public override void WalkExpression(Expression expression)
            {
                if (!this.types.ContainsKey(expression))
                {
                    base.WalkExpression(expression);
                    this.changed = this.changed || this.types.ContainsKey(expression);
                }
                else
                {
                    base.WalkExpression(expression);
                }
            }

            public override void WalkGrammar(Grammar grammar)
            {
                this.changed = false;
                base.WalkGrammar(grammar);
            }

            protected override void WalkAndCodeExpression(AndCodeExpression andCodeExpression)
            {
                this.Set(andCodeExpression, "string");
            }

            protected override void WalkAndExpression(AndExpression andExpression)
            {
                base.WalkAndExpression(andExpression);
                this.Set(andExpression, andExpression.Expression);
            }

            protected override void WalkChoiceExpression(ChoiceExpression choiceExpression)
            {
                base.WalkChoiceExpression(choiceExpression);
                this.Set(choiceExpression, choiceExpression.Choices[0]);
            }

            protected override void WalkClassExpression(ClassExpression classExpression)
            {
                this.Set(classExpression, "string");
            }

            protected override void WalkCodeExpression(CodeExpression codeExpression)
            {
                this.Set(codeExpression, "string");
            }

            protected override void WalkLiteralExpression(LiteralExpression literalExpression)
            {
                this.Set(literalExpression, "string");
            }

            protected override void WalkNameExpression(NameExpression nameExpression)
            {
                var rule = this.rules[nameExpression.Identifier.Name];
                this.Set(nameExpression, rule.Expression, type => type.ToString());
            }

            protected override void WalkNotCodeExpression(NotCodeExpression notCodeExpression)
            {
                this.Set(notCodeExpression, "string");
            }

            protected override void WalkNotExpression(NotExpression notExpression)
            {
                base.WalkNotExpression(notExpression);
                this.Set(notExpression, "string");
            }

            protected override void WalkPrefixedExpression(PrefixedExpression prefixedExpression)
            {
                base.WalkPrefixedExpression(prefixedExpression);
                this.Set(prefixedExpression, prefixedExpression.Expression);
            }

            protected override void WalkRepetitionExpression(RepetitionExpression repetitionExpression)
            {
                base.WalkRepetitionExpression(repetitionExpression);
                this.Set(repetitionExpression, repetitionExpression.Expression, type => "IList<" + type + ">");
            }

            protected override void WalkSequenceExpression(SequenceExpression sequenceExpression)
            {
                base.WalkSequenceExpression(sequenceExpression);
                this.Set(sequenceExpression, "string");
            }

            protected override void WalkTypedExpression(TypedExpression typedExpression)
            {
                base.WalkTypedExpression(typedExpression);
                this.Set(typedExpression, typedExpression.Type);
            }

            protected override void WalkWildcardExpression(WildcardExpression wildcardExpression)
            {
                this.Set(wildcardExpression, "string");
            }

            private void Set(Expression expression, object value)
            {
                if (!this.types.ContainsKey(expression))
                {
                    this.types[expression] = value;
                }
            }

            private void Set(Expression expression, Expression toCopy, Func<object, object> map = null)
            {
                map = map ?? (t => t);

                if (!this.types.ContainsKey(expression) && this.types.TryGetValue(toCopy, out var type))
                {
                    this.types[expression] = map(type);
                }
            }
        }
    }
}
