// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System;
    using Pegasus.Expressions;

    internal abstract class ExpressionTreeWalker
    {
        private Action<Expression> expressionDispatcher;

        public ExpressionTreeWalker()
        {
            this.expressionDispatcher = ExpressionDispatch.CreateDispatcher(
                And: this.WalkAndExpression,
                AndCode: this.WalkAndCodeExpression,
                Choice: this.WalkChoiceExpression,
                Class: this.WalkClassExpression,
                Code: this.WalkCodeExpression,
                Literal: this.WalkLiteralExpression,
                Name: this.WalkNameExpression,
                Not: this.WalkNotExpression,
                NotCode: this.WalkNotCodeExpression,
                Prefixed: this.WalkPrefixedExpression,
                Repetition: this.WalkRepetitionExpression,
                Sequence: this.WalkSequenceExpression,
                Typed: this.WalkTypedExpression,
                Wildcard: this.WalkWildcardExpression);
        }

        public virtual void WalkExpression(Expression expression) => this.expressionDispatcher(expression);

        public virtual void WalkGrammar(Grammar grammar)
        {
            foreach (var rule in grammar.Rules)
            {
                this.WalkRule(rule);
            }
        }

        protected virtual void WalkAndCodeExpression(AndCodeExpression andCodeExpression)
        {
        }

        protected virtual void WalkAndExpression(AndExpression andExpression)
        {
            this.WalkExpression(andExpression.Expression);
        }

        protected virtual void WalkChoiceExpression(ChoiceExpression choiceExpression)
        {
            foreach (var expression in choiceExpression.Choices)
            {
                this.WalkExpression(expression);
            }
        }

        protected virtual void WalkClassExpression(ClassExpression classExpression)
        {
        }

        protected virtual void WalkCodeExpression(CodeExpression codeExpression)
        {
        }

        protected virtual void WalkLiteralExpression(LiteralExpression literalExpression)
        {
        }

        protected virtual void WalkNameExpression(NameExpression nameExpression)
        {
        }

        protected virtual void WalkNotCodeExpression(NotCodeExpression notCodeExpression)
        {
        }

        protected virtual void WalkNotExpression(NotExpression notExpression)
        {
            this.WalkExpression(notExpression.Expression);
        }

        protected virtual void WalkPrefixedExpression(PrefixedExpression prefixedExpression)
        {
            this.WalkExpression(prefixedExpression.Expression);
        }

        protected virtual void WalkRepetitionExpression(RepetitionExpression repetitionExpression)
        {
            this.WalkExpression(repetitionExpression.Expression);

            if (repetitionExpression.Quantifier.Delimiter != null)
            {
                this.WalkExpression(repetitionExpression.Quantifier.Delimiter);
            }
        }

        protected virtual void WalkRule(Rule rule)
        {
            this.WalkExpression(rule.Expression);
        }

        protected virtual void WalkSequenceExpression(SequenceExpression sequenceExpression)
        {
            foreach (var expression in sequenceExpression.Sequence)
            {
                this.WalkExpression(expression);
            }
        }

        protected virtual void WalkTypedExpression(TypedExpression typedExpression)
        {
            this.WalkExpression(typedExpression.Expression);
        }

        protected virtual void WalkWildcardExpression(WildcardExpression wildcardExpression)
        {
        }
    }
}
