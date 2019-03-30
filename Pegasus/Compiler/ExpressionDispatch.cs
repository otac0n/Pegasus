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
                switch (expression)
                {
                    case AndExpression andExpression:
                        And?.Invoke(andExpression);
                        break;
                    case AndCodeExpression andCodeExpression:
                        AndCode?.Invoke(andCodeExpression);
                        break;
                    case ChoiceExpression choiceExpression:
                        Choice?.Invoke(choiceExpression);
                        break;
                    case ClassExpression classExpression:
                        Class?.Invoke(classExpression);
                        break;
                    case CodeExpression codeExpression:
                        Code?.Invoke(codeExpression);
                        break;
                    case LiteralExpression literalExpression:
                        Literal?.Invoke(literalExpression);
                        break;
                    case NameExpression nameExpression:
                        Name?.Invoke(nameExpression);
                        break;
                    case NotExpression notExpression:
                        Not?.Invoke(notExpression);
                        break;
                    case NotCodeExpression notCodeExpression:
                        NotCode?.Invoke(notCodeExpression);
                        break;
                    case PrefixedExpression prefixedExpression:
                        Prefixed?.Invoke(prefixedExpression);
                        break;
                    case RepetitionExpression repetitionExpression:
                        Repetition?.Invoke(repetitionExpression);
                        break;
                    case SequenceExpression sequenceExpression:
                        Sequence?.Invoke(sequenceExpression);
                        break;
                    case TypedExpression typedExpression:
                        Typed?.Invoke(typedExpression);
                        break;
                    case WildcardExpression wildcardExpression:
                        Wildcard?.Invoke(wildcardExpression);
                        break;
                    default:
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Unknown expression type '{0}'.", expression.GetType()), nameof(expression));
                }
            };
        }
    }
}
