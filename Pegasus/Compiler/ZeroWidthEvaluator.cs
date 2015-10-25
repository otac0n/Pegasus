// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Pegasus.Expressions;

    /// <summary>
    /// Performs zero-width evaluation services for Pegasus <see cref="Grammar">Grammars</see>.
    /// </summary>
    public static class ZeroWidthEvaluator
    {
        /// <summary>
        /// Evaluates the expressions in a Pegasus <see cref="Grammar"/> to determine which of them are zero-width.
        /// </summary>
        /// <param name="grammar">The <see cref="Grammar"/> to evaluate.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> detailing the whether each expression in the grammar is zero-width.</returns>
        public static Dictionary<Expression, bool> Evaluate(Grammar grammar)
        {
            var zeroWidth = new Dictionary<Expression, bool?>();
            var walker = new ZeroWidthWalker(grammar, zeroWidth);

            do
            {
                walker.WalkGrammar(grammar);
            }
            while (walker.Changed);

            return zeroWidth.ToDictionary(x => x.Key, x => x.Value ?? false);
        }

        private class ZeroWidthWalker : ExpressionTreeWalker
        {
            private readonly Dictionary<string, Rule> rules;
            private readonly Dictionary<Expression, bool?> zeroWidth;
            private bool changed;

            public ZeroWidthWalker(Grammar grammar, Dictionary<Expression, bool?> zeroWidth)
            {
                this.rules = grammar.Rules.ToDictionary(r => r.Identifier.Name);
                this.zeroWidth = zeroWidth;
            }

            public bool Changed => this.changed;

            public override void WalkExpression(Expression expression)
            {
                bool? starting;
                if (!this.zeroWidth.TryGetValue(expression, out starting))
                {
                    this.zeroWidth[expression] = starting;
                }

                base.WalkExpression(expression);
                this.changed = this.changed || starting != this.zeroWidth[expression];
            }

            public override void WalkGrammar(Grammar grammar)
            {
                this.changed = false;
                base.WalkGrammar(grammar);
            }

            protected override void WalkAndCodeExpression(AndCodeExpression andCodeExpression)
            {
                this.zeroWidth[andCodeExpression] = true;
            }

            protected override void WalkAndExpression(AndExpression andExpression)
            {
                base.WalkAndExpression(andExpression);

                this.zeroWidth[andExpression] = true;
            }

            protected override void WalkChoiceExpression(ChoiceExpression choiceExpression)
            {
                base.WalkChoiceExpression(choiceExpression);

                bool? result = false;
                foreach (var e in choiceExpression.Choices)
                {
                    result |= this.zeroWidth[e];

                    if (result == true)
                    {
                        break;
                    }
                }

                this.zeroWidth[choiceExpression] = result;
            }

            protected override void WalkClassExpression(ClassExpression classExpression)
            {
                this.zeroWidth[classExpression] = false;
            }

            protected override void WalkCodeExpression(CodeExpression codeExpression)
            {
                switch (codeExpression.CodeType)
                {
                    case CodeType.Result:
                    case CodeType.State:
                        this.zeroWidth[codeExpression] = true;
                        break;

                    case CodeType.Error:
                        this.zeroWidth[codeExpression] = false;
                        break;

                    default:
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unknown code type '{0}'.", codeExpression.CodeType));
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Empty strings and null strings are logically distinct in this method.")]
            protected override void WalkLiteralExpression(LiteralExpression literalExpression)
            {
                this.zeroWidth[literalExpression] = literalExpression.Value == string.Empty;
            }

            protected override void WalkNameExpression(NameExpression nameExpression)
            {
                bool? result;
                this.zeroWidth.TryGetValue(this.rules[nameExpression.Identifier.Name].Expression, out result);
                this.zeroWidth[nameExpression] = result;
            }

            protected override void WalkNotCodeExpression(NotCodeExpression notCodeExpression)
            {
                this.zeroWidth[notCodeExpression] = true;
            }

            protected override void WalkNotExpression(NotExpression notExpression)
            {
                base.WalkNotExpression(notExpression);

                this.zeroWidth[notExpression] = true;
            }

            protected override void WalkPrefixedExpression(PrefixedExpression prefixedExpression)
            {
                base.WalkPrefixedExpression(prefixedExpression);

                this.zeroWidth[prefixedExpression] = this.zeroWidth[prefixedExpression.Expression];
            }

            protected override void WalkRepetitionExpression(RepetitionExpression repetitionExpression)
            {
                base.WalkRepetitionExpression(repetitionExpression);

                bool? result;
                if (repetitionExpression.Quantifier.Min == 0)
                {
                    result = true;
                }
                else if (repetitionExpression.Quantifier.Min <= 1 || repetitionExpression.Quantifier.Delimiter == null)
                {
                    result = this.zeroWidth[repetitionExpression.Expression];
                }
                else
                {
                    result = this.zeroWidth[repetitionExpression.Expression] & this.zeroWidth[repetitionExpression.Quantifier.Delimiter];
                }

                this.zeroWidth[repetitionExpression] = result;
            }

            protected override void WalkSequenceExpression(SequenceExpression sequenceExpression)
            {
                base.WalkSequenceExpression(sequenceExpression);

                bool? result = true;
                foreach (var e in sequenceExpression.Sequence)
                {
                    result &= this.zeroWidth[e];

                    if (result == false)
                    {
                        break;
                    }
                }

                this.zeroWidth[sequenceExpression] = result;
            }

            protected override void WalkTypedExpression(TypedExpression typedExpression)
            {
                base.WalkTypedExpression(typedExpression);

                this.zeroWidth[typedExpression] = this.zeroWidth[typedExpression.Expression];
            }

            protected override void WalkWildcardExpression(WildcardExpression wildcardExpression)
            {
                base.WalkWildcardExpression(wildcardExpression);

                this.zeroWidth[wildcardExpression] = false;
            }
        }
    }
}
