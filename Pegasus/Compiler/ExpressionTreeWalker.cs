// -----------------------------------------------------------------------
// <copyright file="ExpressionTreeWalker.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System;
    using System.Globalization;
    using Pegasus.Expressions;

    internal abstract class ExpressionTreeWalker
    {
        public virtual void WalkExpression(Expression expression)
        {
            AndCodeExpression andCodeExpression;
            AndExpression andExpression;
            ChoiceExpression choiceExpression;
            ClassExpression classExpression;
            CodeExpression codeExpression;
            LiteralExpression literalExpression;
            NameExpression nameExpression;
            NotCodeExpression notCodeExpression;
            NotExpression notExpression;
            PrefixedExpression prefixedExpression;
            RepetitionExpression repetitionExpression;
            SequenceExpression sequenceExpression;
            TypedExpression typedExpression;
            WildcardExpression wildcardExpression;

            if ((andCodeExpression = expression as AndCodeExpression) != null)
            {
                this.WalkAndCodeExpression(andCodeExpression);
            }
            else if ((andExpression = expression as AndExpression) != null)
            {
                this.WalkAndExpression(andExpression);
            }
            else if ((choiceExpression = expression as ChoiceExpression) != null)
            {
                this.WalkChoiceExpression(choiceExpression);
            }
            else if ((classExpression = expression as ClassExpression) != null)
            {
                this.WalkClassExpression(classExpression);
            }
            else if ((codeExpression = expression as CodeExpression) != null)
            {
                this.WalkCodeExpression(codeExpression);
            }
            else if ((literalExpression = expression as LiteralExpression) != null)
            {
                this.WalkLiteralExpression(literalExpression);
            }
            else if ((nameExpression = expression as NameExpression) != null)
            {
                this.WalkNameExpression(nameExpression);
            }
            else if ((notCodeExpression = expression as NotCodeExpression) != null)
            {
                this.WalkNotCodeExpression(notCodeExpression);
            }
            else if ((notExpression = expression as NotExpression) != null)
            {
                this.WalkNotExpression(notExpression);
            }
            else if ((prefixedExpression = expression as PrefixedExpression) != null)
            {
                this.WalkPrefixedExpression(prefixedExpression);
            }
            else if ((repetitionExpression = expression as RepetitionExpression) != null)
            {
                this.WalkRepetitionExpression(repetitionExpression);
            }
            else if ((sequenceExpression = expression as SequenceExpression) != null)
            {
                this.WalkSequenceExpression(sequenceExpression);
            }
            else if ((typedExpression = expression as TypedExpression) != null)
            {
                this.WalkTypedExpression(typedExpression);
            }
            else if ((wildcardExpression = expression as WildcardExpression) != null)
            {
                this.WalkWildcardExpression(wildcardExpression);
            }
            else
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Unknown expression type '{0}'.", expression.GetType()), "expression");
            }
        }

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
