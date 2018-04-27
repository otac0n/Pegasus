// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Performs assertion detection services for Pegasus <see cref="Grammar">Grammars</see>.
    /// </summary>
    public static class ContainsAssertionsEvaluator
    {
        /// <summary>
        /// Evaluates the expressions in a Pegasus <see cref="Grammar"/> to determine which of them contain assertions.
        /// </summary>
        /// <param name="grammar">The <see cref="Grammar"/> to evaluate.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> detailing the whether each expression in the grammar contains assertions.</returns>
        public static Dictionary<Expression, bool> Evaluate(Grammar grammar)
        {
            var containsAssertions = new Dictionary<Expression, bool?>();
            var walker = new ContainsAssertionsWalker(grammar, containsAssertions);

            do
            {
                walker.WalkGrammar(grammar);
            }
            while (walker.Changed);

            return containsAssertions.ToDictionary(x => x.Key, x => x.Value ?? false);
        }

        private class ContainsAssertionsWalker : ExpressionTreeWalker
        {
            private readonly Dictionary<Expression, bool?> containsAssertions;
            private readonly Dictionary<string, Rule> rules;
            private bool changed;

            public ContainsAssertionsWalker(Grammar grammar, Dictionary<Expression, bool?> containsAssertions)
            {
                this.rules = grammar.Rules.ToDictionary(r => r.Identifier.Name);
                this.containsAssertions = containsAssertions;
            }

            public bool Changed => this.changed;

            public override void WalkExpression(Expression expression)
            {
                if (!this.containsAssertions.TryGetValue(expression, out var starting))
                {
                    this.containsAssertions[expression] = starting;
                }

                base.WalkExpression(expression);
                this.changed = this.changed || starting != this.containsAssertions[expression];
            }

            public override void WalkGrammar(Grammar grammar)
            {
                this.changed = false;
                base.WalkGrammar(grammar);
            }

            protected override void WalkAndCodeExpression(AndCodeExpression andCodeExpression)
            {
                this.containsAssertions[andCodeExpression] = true;
            }

            protected override void WalkAndExpression(AndExpression andExpression)
            {
                base.WalkAndExpression(andExpression);

                this.containsAssertions[andExpression] = true;
            }

            protected override void WalkChoiceExpression(ChoiceExpression choiceExpression)
            {
                base.WalkChoiceExpression(choiceExpression);

                bool? result = false;
                foreach (var e in choiceExpression.Choices)
                {
                    result |= this.containsAssertions[e];

                    if (result == true)
                    {
                        break;
                    }
                }

                this.containsAssertions[choiceExpression] = result;
            }

            protected override void WalkClassExpression(ClassExpression classExpression)
            {
                this.containsAssertions[classExpression] = false;
            }

            protected override void WalkCodeExpression(CodeExpression codeExpression)
            {
                this.containsAssertions[codeExpression] = codeExpression.CodeType == CodeType.Parse;
            }

            protected override void WalkLiteralExpression(LiteralExpression literalExpression)
            {
                this.containsAssertions[literalExpression] = false;
            }

            protected override void WalkNameExpression(NameExpression nameExpression)
            {
                this.containsAssertions.TryGetValue(this.rules[nameExpression.Identifier.Name].Expression, out var result);
                this.containsAssertions[nameExpression] = result;
            }

            protected override void WalkNotCodeExpression(NotCodeExpression notCodeExpression)
            {
                this.containsAssertions[notCodeExpression] = true;
            }

            protected override void WalkNotExpression(NotExpression notExpression)
            {
                base.WalkNotExpression(notExpression);

                this.containsAssertions[notExpression] = true;
            }

            protected override void WalkPrefixedExpression(PrefixedExpression prefixedExpression)
            {
                base.WalkPrefixedExpression(prefixedExpression);

                this.containsAssertions[prefixedExpression] = this.containsAssertions[prefixedExpression.Expression];
            }

            protected override void WalkRepetitionExpression(RepetitionExpression repetitionExpression)
            {
                base.WalkRepetitionExpression(repetitionExpression);

                var result = this.containsAssertions[repetitionExpression.Expression];
                if (repetitionExpression.Quantifier.Delimiter != null)
                {
                    result |= this.containsAssertions[repetitionExpression.Quantifier.Delimiter];
                }

                this.containsAssertions[repetitionExpression] = result;
            }

            protected override void WalkSequenceExpression(SequenceExpression sequenceExpression)
            {
                base.WalkSequenceExpression(sequenceExpression);

                bool? result = false;
                foreach (var e in sequenceExpression.Sequence)
                {
                    result |= this.containsAssertions[e];

                    if (result == true)
                    {
                        break;
                    }
                }

                this.containsAssertions[sequenceExpression] = result;
            }

            protected override void WalkTypedExpression(TypedExpression typedExpression)
            {
                base.WalkTypedExpression(typedExpression);

                this.containsAssertions[typedExpression] = this.containsAssertions[typedExpression.Expression];
            }

            protected override void WalkWildcardExpression(WildcardExpression wildcardExpression)
            {
                base.WalkWildcardExpression(wildcardExpression);

                this.containsAssertions[wildcardExpression] = false;
            }
        }
    }
}
