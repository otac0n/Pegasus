// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Compiler
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Pegasus.Expressions;

    /// <summary>
    /// Provides expression dispatch.
    /// </summary>
    public static class ExpressionDispatch
    {
        /// <summary>
        /// Creates an expression dispatcher.
        /// </summary>
        /// <param name="And">The action to be performed in the case of a <see cref="AndExpression"/></param>
        /// <param name="AndCode">The action to be performed in the case of a <see cref="AndCodeExpression"/></param>
        /// <param name="Choice">The action to be performed in the case of a <see cref="ChoiceExpression"/></param>
        /// <param name="Class">The action to be performed in the case of a <see cref="ClassExpression"/></param>
        /// <param name="Code">The action to be performed in the case of a <see cref="CodeExpression"/></param>
        /// <param name="Literal">The action to be performed in the case of a <see cref="LiteralExpression"/></param>
        /// <param name="Name">The action to be performed in the case of a <see cref="NameExpression"/></param>
        /// <param name="Not">The action to be performed in the case of a <see cref="NotExpression"/></param>
        /// <param name="NotCode">The action to be performed in the case of a <see cref="NotCodeExpression"/></param>
        /// <param name="Prefixed">The action to be performed in the case of a <see cref="PrefixedExpression"/></param>
        /// <param name="Repetition">The action to be performed in the case of a <see cref="RepetitionExpression"/></param>
        /// <param name="Sequence">The action to be performed in the case of a <see cref="SequenceExpression"/></param>
        /// <param name="Typed">The action to be performed in the case of a <see cref="TypedExpression"/></param>
        /// <param name="Wildcard">The action to be performed in the case of a <see cref="WildcardExpression"/></param>
        /// <returns>The expression dispatcher that will take perform specified action for a given <see cref="Expression"/>.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This function is not actually complex.")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "In order to avoid keyword collision and to maintain a consistent API, these are intentionally pascal-cased.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:ParameterNamesMustBeginWithLowerCaseLetter", Justification = "In order to avoid keyword collision and to maintain a consistent API, these are intentionally pascal-cased.")]
        public static Action<Expression> CreateDispatcher(
            Action<AndExpression> And = null,
            Action<AndCodeExpression> AndCode = null,
            Action<ChoiceExpression> Choice = null,
            Action<ClassExpression> Class = null,
            Action<CodeExpression> Code = null,
            Action<LiteralExpression> Literal = null,
            Action<NameExpression> Name = null,
            Action<NotExpression> Not = null,
            Action<NotCodeExpression> NotCode = null,
            Action<PrefixedExpression> Prefixed = null,
            Action<RepetitionExpression> Repetition = null,
            Action<SequenceExpression> Sequence = null,
            Action<TypedExpression> Typed = null,
            Action<WildcardExpression> Wildcard = null)
        {
            return expression =>
            {
                AndExpression andExpression;
                AndCodeExpression andCodeExpression;
                ChoiceExpression choiceExpression;
                ClassExpression classExpression;
                CodeExpression codeExpression;
                LiteralExpression literalExpression;
                NameExpression nameExpression;
                NotExpression notExpression;
                NotCodeExpression notCodeExpression;
                PrefixedExpression prefixedExpression;
                RepetitionExpression repetitionExpression;
                SequenceExpression sequenceExpression;
                TypedExpression typedExpression;
                WildcardExpression wildcardExpression;

                if ((andExpression = expression as AndExpression) != null)
                {
                    if (And != null)
                    {
                        And(andExpression);
                    }
                }
                else if ((andCodeExpression = expression as AndCodeExpression) != null)
                {
                    if (AndCode != null)
                    {
                        AndCode(andCodeExpression);
                    }
                }
                else if ((choiceExpression = expression as ChoiceExpression) != null)
                {
                    if (Choice != null)
                    {
                        Choice(choiceExpression);
                    }
                }
                else if ((classExpression = expression as ClassExpression) != null)
                {
                    if (Class != null)
                    {
                        Class(classExpression);
                    }
                }
                else if ((codeExpression = expression as CodeExpression) != null)
                {
                    if (Code != null)
                    {
                        Code(codeExpression);
                    }
                }
                else if ((literalExpression = expression as LiteralExpression) != null)
                {
                    if (Literal != null)
                    {
                        Literal(literalExpression);
                    }
                }
                else if ((nameExpression = expression as NameExpression) != null)
                {
                    if (Name != null)
                    {
                        Name(nameExpression);
                    }
                }
                else if ((notExpression = expression as NotExpression) != null)
                {
                    if (Not != null)
                    {
                        Not(notExpression);
                    }
                }
                else if ((notCodeExpression = expression as NotCodeExpression) != null)
                {
                    if (NotCode != null)
                    {
                        NotCode(notCodeExpression);
                    }
                }
                else if ((prefixedExpression = expression as PrefixedExpression) != null)
                {
                    if (Prefixed != null)
                    {
                        Prefixed(prefixedExpression);
                    }
                }
                else if ((repetitionExpression = expression as RepetitionExpression) != null)
                {
                    if (Repetition != null)
                    {
                        Repetition(repetitionExpression);
                    }
                }
                else if ((sequenceExpression = expression as SequenceExpression) != null)
                {
                    if (Sequence != null)
                    {
                        Sequence(sequenceExpression);
                    }
                }
                else if ((typedExpression = expression as TypedExpression) != null)
                {
                    if (Typed != null)
                    {
                        Typed(typedExpression);
                    }
                }
                else if ((wildcardExpression = expression as WildcardExpression) != null)
                {
                    if (Wildcard != null)
                    {
                        Wildcard(wildcardExpression);
                    }
                }
                else
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Unknown expression type '{0}'.", expression.GetType()), "expression");
                }
            };
        }
    }
}
