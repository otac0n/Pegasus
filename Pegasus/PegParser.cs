// -----------------------------------------------------------------------
// <copyright file="PegParser.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Pegasus.Expressions;

    public class PegParser
    {
        public ParseResult<Grammar> Parse(string subject, int location = 0)
        {
            return this.ParseGrammar(new Cursor(subject, location));
        }

        private ParseResult<Grammar> ParseGrammar(Cursor cursor)
        {
            var startCursor = cursor;

            var ws1 = this.ParseWs(cursor);
            if (ws1 != null)
            {
                cursor = cursor.Advance(ws1);
            }
            else
            {
                return null;
            }

            var initializer1 = this.ParseInitializer(cursor);
            if (initializer1 != null)
            {
                cursor = cursor.Advance(initializer1);
            }

            var rules = new List<Rule>();
            while (true)
            {
                var rule1 = this.ParseRule(cursor);
                if (rule1 != null)
                {
                    cursor = cursor.Advance(rule1);
                    rules.Add(rule1.Value);
                }
                else
                {
                    break;
                }
            }

            if (rules.Count == 0)
            {
                return null;
            }

            var len = cursor - startCursor;
            return new ParseResult<Grammar>(len, new Grammar(rules, initializer1 != null ? initializer1.Value : null));
        }

        private ParseResult<string> ParseInitializer(Cursor cursor)
        {
            var startCursor = cursor;

            var action1 = this.ParseAction(cursor);
            if (action1 != null)
            {
                cursor = cursor.Advance(action1);

                var semicolon1 = this.ParseSemicolon(cursor);
                if (semicolon1 != null)
                {
                    cursor = cursor.Advance(semicolon1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, ((CodeExpression)action1.Value).Code);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Rule> ParseRule(Cursor cursor)
        {
            var startCursor = cursor;

            var identifier1 = this.ParseIdentifier(cursor);
            if (identifier1 != null)
            {
                cursor = cursor.Advance(identifier1);

                var string1 = this.ParseString(cursor);
                if (string1 != null)
                {
                    cursor = cursor.Advance(string1);
                }

                var equals1 = this.ParseEquals(cursor);
                if (equals1 != null)
                {
                    cursor = cursor.Advance(equals1);

                    var expression1 = this.ParseExpression(cursor);
                    if (expression1 != null)
                    {
                        cursor = cursor.Advance(expression1);

                        var semicolon1 = this.ParseSemicolon(cursor);
                        if (semicolon1 != null)
                        {
                            cursor = cursor.Advance(semicolon1);
                        }

                        var len = cursor - startCursor;
                        return new ParseResult<Rule>(len, new Rule(identifier1.Value, string1 != null ? string1.Value : null, expression1.Value));
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Expression> ParseExpression(Cursor cursor)
        {
            var startCursor = cursor;

            var type1 = this.ParseType(cursor);
            if (type1 != null)
            {
                cursor = cursor.Advance(type1);
            }

            var choice1 = this.ParseChoice(cursor);
            if (choice1 != null)
            {
                cursor = cursor.Advance(choice1);

                var len = cursor - startCursor;
                return new ParseResult<Expression>(len, type1 != null
                    ? new TypedExpression(type1.Value, choice1.Value)
                    : choice1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseType(Cursor cursor)
        {
            var startCursor = cursor;

            var lt1 = this.ParseLt(cursor);
            if (lt1 != null)
            {
                cursor = cursor.Advance(lt1);

                var dotted1 = this.ParseDotted(cursor);
                if (dotted1 != null)
                {
                    cursor = cursor.Advance(dotted1);

                    var gt1 = this.ParseGt(cursor);
                    if (gt1 != null)
                    {
                        cursor = cursor.Advance(gt1);

                        var len = cursor - startCursor;
                        return new ParseResult<string>(len, dotted1.Value);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Expression> ParseChoice(Cursor cursor)
        {
            var startCursor = cursor;

            var sequence1 = this.ParseSequence(cursor);
            if (sequence1 != null)
            {
                cursor = cursor.Advance(sequence1);

                var tail = new List<Expression>();
                while (true)
                {
                    var startCursor2 = cursor;

                    var slash1 = this.ParseSlash(cursor);
                    if (slash1 != null)
                    {
                        cursor = cursor.Advance(slash1);
                    }
                    else
                    {
                        break;
                    }

                    var sequence2 = this.ParseSequence(cursor);
                    if (sequence2 != null)
                    {
                        cursor = cursor.Advance(sequence2);

                        tail.Add(sequence2.Value);
                    }
                    else
                    {
                        cursor = startCursor2;
                        break;
                    }
                }

                var len = cursor - startCursor;
                if (tail.Count == 0)
                {
                    return new ParseResult<Expression>(len, sequence1.Value);
                }
                else
                {
                    return new ParseResult<Expression>(len, new ChoiceExpression(Enumerable.Concat(new[] { sequence1.Value }, tail)));
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Expression> ParseSequence(Cursor cursor)
        {
            var startCursor = cursor;

            var elements = new List<Expression>();
            while (true)
            {
                var labeled1 = this.ParseLabeled(cursor);
                if (labeled1 != null)
                {
                    cursor = cursor.Advance(labeled1);

                    elements.Add(labeled1.Value);
                }
                else
                {
                    break;
                }
            }

            var action1 = this.ParseAction(cursor);
            if (action1 != null)
            {
                cursor = cursor.Advance(action1);

                elements.Add(action1.Value);
            }

            var len = cursor - startCursor;
            if (elements.Count == 1)
            {
                return new ParseResult<Expression>(len, elements[0]);
            }
            else
            {
                return new ParseResult<Expression>(len, new SequenceExpression(elements));
            }
        }

        private ParseResult<Expression> ParseLabeled(Cursor cursor)
        {
            var startCursor = cursor;

            var identifier1 = this.ParseIdentifier(cursor);
            if (identifier1 != null)
            {
                cursor = cursor.Advance(identifier1);

                var colon1 = this.ParseColon(cursor);
                if (colon1 != null)
                {
                    cursor = cursor.Advance(colon1);

                    var prefixed1 = this.ParsePrefixed(cursor);
                    if (prefixed1 != null)
                    {
                        cursor = cursor.Advance(prefixed1);

                        var len = cursor - startCursor;
                        return new ParseResult<Expression>(len, new PrefixedExpression(identifier1.Value, prefixed1.Value));
                    }
                    else
                    {
                        cursor = startCursor;
                    }
                }
                else
                {
                    cursor = startCursor;
                }
            }

            var prefixed2 = this.ParsePrefixed(cursor);
            if (prefixed2 != null)
            {
                cursor = cursor.Advance(prefixed2);

                var len = cursor - startCursor;
                return new ParseResult<Expression>(len, prefixed2.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Expression> ParsePrefixed(Cursor cursor)
        {
            var startCursor = cursor;

            var and1 = this.ParseAnd(cursor);
            if (and1 != null)
            {
                cursor = cursor.Advance(and1);

                var suffixed1 = this.ParseSuffixed(cursor);
                if (suffixed1 != null)
                {
                    cursor = cursor.Advance(suffixed1);

                    var len = cursor - startCursor;
                    return new ParseResult<Expression>(len, new AndExpression(suffixed1.Value));
                }
                else
                {
                    cursor = startCursor;
                }
            }

            var not1 = this.ParseNot(cursor);
            if (not1 != null)
            {
                cursor = cursor.Advance(not1);

                var suffixed2 = this.ParseSuffixed(cursor);
                if (suffixed2 != null)
                {
                    cursor = cursor.Advance(suffixed2);

                    var len = cursor - startCursor;
                    return new ParseResult<Expression>(len, new NotExpression(suffixed2.Value));
                }
                else
                {
                    cursor = startCursor;
                }
            }

            var suffixed3 = this.ParseSuffixed(cursor);
            if (suffixed3 != null)
            {
                cursor = cursor.Advance(suffixed3);

                var len = cursor - startCursor;
                return new ParseResult<Expression>(len, suffixed3.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Expression> ParseSuffixed(Cursor cursor)
        {
            var startCursor = cursor;

            var primary1 = this.ParsePrimary(cursor);
            if (primary1 != null)
            {
                cursor = cursor.Advance(primary1);

                var question1 = this.ParseQuestion(cursor);
                if (question1 != null)
                {
                    cursor = cursor.Advance(question1);

                    var len = cursor - startCursor;
                    return new ParseResult<Expression>(len, new RepetitionExpression(primary1.Value, min: 0, max: 1));
                }

                var star1 = this.ParseStar(cursor);
                if (star1 != null)
                {
                    cursor = cursor.Advance(star1);

                    var len = cursor - startCursor;
                    return new ParseResult<Expression>(len, new RepetitionExpression(primary1.Value, min: 0, max: null));
                }

                var plus1 = this.ParsePlus(cursor);
                if (plus1 != null)
                {
                    cursor = cursor.Advance(plus1);

                    var len = cursor - startCursor;
                    return new ParseResult<Expression>(len, new RepetitionExpression(primary1.Value, min: 1, max: null));
                }

                {
                    var len = cursor - startCursor;
                    return new ParseResult<Expression>(len, primary1.Value);
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Expression> ParsePrimary(Cursor cursor)
        {
            var startCursor = cursor;

            var identifier1 = this.ParseIdentifier(cursor);
            if (identifier1 != null)
            {
                cursor = cursor.Advance(identifier1);

                var startCursor1 = cursor;
                var string1 = this.ParseString(cursor);
                if (string1 != null)
                {
                    cursor = cursor.Advance(string1);
                }

                var equals1 = this.ParseEquals(cursor);
                if (equals1 != null)
                {
                    return null;
                }

                cursor = startCursor1;

                var len = cursor - startCursor;
                return new ParseResult<Expression>(len, new NameExpression(identifier1.Value));
            }

            var literal1 = this.ParseLiteral(cursor);
            if (literal1 != null)
            {
                return literal1;
            }

            var class1 = this.ParseClass(cursor);
            if (class1 != null)
            {
                return class1;
            }

            var dot1 = this.ParseDot(cursor);
            if (dot1 != null)
            {
                cursor = cursor.Advance(dot1);

                var len = cursor - startCursor;
                return new ParseResult<Expression>(len, new WildcardExpression());
            }

            var lparen1 = this.ParseLParen(cursor);
            if (lparen1 != null)
            {
                cursor = cursor.Advance(lparen1);

                var expression1 = this.ParseExpression(cursor);
                if (expression1 != null)
                {
                    cursor = cursor.Advance(expression1);

                    var rparen1 = this.ParseRParen(cursor);
                    if (rparen1 != null)
                    {
                        cursor = cursor.Advance(rparen1);

                        var len = cursor - startCursor;
                        return new ParseResult<Expression>(len, expression1.Value);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Expression> ParseAction(Cursor cursor)
        {
            var startCursor = cursor;

            var braced1 = this.ParseBraced(cursor);
            if (braced1 != null)
            {
                cursor = cursor.Advance(braced1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<Expression>(len, new CodeExpression(braced1.Value));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseBraced(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("{", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                while (true)
                {
                    var part =
                        this.ParseNonBracedCharacters(cursor) ??
                        this.ParseBraced(cursor);
                    if (part != null)
                    {
                        cursor = cursor.Advance(part);
                    }
                    else
                    {
                        break;
                    }
                }

                var l2 = this.ParseLiteral("}", cursor);
                if (l2 != null)
                {
                    cursor = cursor.Advance(l2);

                    var len = cursor - startCursor;
                    return new ParseResult<string>(len, cursor.Subject.Substring(startCursor.Location + 1, len - 2));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseNonBracedCharacters(Cursor cursor)
        {
            var startCursor = cursor;

            var chars = new List<string>();
            while (true)
            {
                var nonBraceCharacter1 = this.ParseNonBraceCharacter(cursor);
                if (nonBraceCharacter1 != null)
                {
                    cursor = cursor.Advance(nonBraceCharacter1);

                    chars.Add(nonBraceCharacter1.Value);
                }
                else
                {
                    break;
                }
            }

            if (chars.Count > 1)
            {
                var len = cursor - startCursor;
                return new ParseResult<string>(len, cursor.Subject.Substring(startCursor.Location + 1, len - 2));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseNonBraceCharacter(Cursor cursor)
        {
            return this.ParseRegex("[^{}]", cursor);
        }

        private ParseResult<Expression> ParseClass(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("[", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var l2 = this.ParseLiteral("^", cursor);
                if (l2 != null)
                {
                    cursor = cursor.Advance(l2);
                }

                var parts = new List<CharacterRange>();
                while (true)
                {
                    var range1 =
                        this.ParseClassCharacterRange(cursor) ??
                        this.ParseClassCharacterSingle(cursor);
                    if (range1 != null)
                    {
                        cursor = cursor.Advance(range1);

                        parts.Add(range1.Value);
                    }
                    else
                    {
                        break;
                    }
                }

                var l3 = this.ParseLiteral("]", cursor);
                if (l3 != null)
                {
                    cursor = cursor.Advance(l3);

                    var l4 = this.ParseLiteral("i", cursor);
                    if (l4 != null)
                    {
                        cursor = cursor.Advance(l4);
                    }

                    var ws1 = this.ParseWs(cursor);
                    if (ws1 != null)
                    {
                        cursor = cursor.Advance(ws1);
                    }
                    else
                    {
                        return null;
                    }

                    var len = cursor - startCursor;
                    return new ParseResult<Expression>(len, new ClassExpression(parts, negated: l2 != null, ignoreCase: l4 != null));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<CharacterRange> ParseClassCharacterRange(Cursor cursor)
        {
            var startCursor = cursor;

            var classCharacter1 = this.ParseClassCharacter(cursor);
            if (classCharacter1 != null)
            {
                cursor = cursor.Advance(classCharacter1);

                var l1 = this.ParseLiteral("-", cursor);
                if (l1 != null)
                {
                    cursor = cursor.Advance(l1);

                    var classCharacter2 = this.ParseClassCharacter(cursor);
                    if (classCharacter2 != null)
                    {
                        cursor = cursor.Advance(classCharacter2);

                        var len = cursor - startCursor;
                        return new ParseResult<CharacterRange>(len, new CharacterRange(classCharacter1.Value, classCharacter2.Value));
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<CharacterRange> ParseClassCharacterSingle(Cursor cursor)
        {
            var startCursor = cursor;

            var classCharacter1 = this.ParseClassCharacter(cursor);
            if (classCharacter1 != null)
            {
                cursor = cursor.Advance(classCharacter1);

                var len = cursor - startCursor;
                return new ParseResult<CharacterRange>(len, new CharacterRange(classCharacter1.Value, classCharacter1.Value));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<char> ParseClassCharacter(Cursor cursor)
        {
            return
                this.ParseSimpleBracketDelimitedCharacter(cursor) ??
                this.ParseSimpleEscapeSequence(cursor) ??
                this.ParseZeroEscapeSequence(cursor) ??
                this.ParseHexEscapeSequence(cursor) ??
                this.ParseUnicodeEscapeSequence(cursor) ??
                this.ParseEolEscapeSequence(cursor);
        }

        private ParseResult<char> ParseSimpleBracketDelimitedCharacter(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 =
                this.ParseLiteral("]", cursor) ??
                this.ParseLiteral("\\", cursor) ??
                this.ParseEolChar(cursor);
            if (l1 != null)
            {
                return null;
            }

            var any1 = this.ParseAny(cursor);
            if (any1 != null)
            {
                cursor = cursor.Advance(any1);

                var len = cursor - startCursor;
                return new ParseResult<char>(len, any1.Value[0]);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<Expression> ParseLiteral(Cursor cursor)
        {
            var startCursor = cursor;

            var string1 =
                this.ParseSingleQuotedString(cursor) ??
                this.ParseDoubleQuotedString(cursor);

            if (string1 != null)
            {
                cursor = cursor.Advance(string1);

                var l1 = this.ParseLiteral("i", cursor);
                if (l1 != null)
                {
                    cursor = cursor.Advance(l1);
                }

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }
                else
                {
                    return null;
                }

                var len = cursor - startCursor;
                return new ParseResult<Expression>(len, new LiteralExpression(string1.Value, ignoreCase: l1 != null));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseString(Cursor cursor)
        {
            var startCursor = cursor;

            var string1 =
                this.ParseSingleQuotedString(cursor) ??
                this.ParseDoubleQuotedString(cursor);

            if (string1 != null)
            {
                cursor = cursor.Advance(string1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);

                    var len = cursor - startCursor;
                    return new ParseResult<string>(len, string1.Value);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseSingleQuotedString(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("'", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var chars = new List<char>();
                while (true)
                {
                    var char1 =
                        this.ParseSimpleSingleQuotedCharacter(cursor) ??
                        this.ParseSimpleEscapeSequence(cursor) ??
                        this.ParseZeroEscapeSequence(cursor) ??
                        this.ParseHexEscapeSequence(cursor) ??
                        this.ParseUnicodeEscapeSequence(cursor) ??
                        this.ParseEolEscapeSequence(cursor);

                    if (char1 != null)
                    {
                        cursor = cursor.Advance(char1);

                        chars.Add(char1.Value);
                    }
                    else
                    {
                        break;
                    }
                }

                var l2 = this.ParseLiteral("'", cursor);
                if (l2 != null)
                {
                    cursor = cursor.Advance(l2);
                }
                else
                {
                    return null;
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, string.Join(string.Empty, chars));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseDoubleQuotedString(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("\"", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var chars = new List<char>();
                while (true)
                {
                    var char1 =
                        this.ParseSimpleDoubleQuotedCharacter(cursor) ??
                        this.ParseSimpleEscapeSequence(cursor) ??
                        this.ParseZeroEscapeSequence(cursor) ??
                        this.ParseHexEscapeSequence(cursor) ??
                        this.ParseUnicodeEscapeSequence(cursor) ??
                        this.ParseEolEscapeSequence(cursor);

                    if (char1 != null)
                    {
                        cursor = cursor.Advance(char1);

                        chars.Add(char1.Value);
                    }
                    else
                    {
                        break;
                    }
                }

                var l2 = this.ParseLiteral("\"", cursor);
                if (l2 != null)
                {
                    cursor = cursor.Advance(l2);
                }
                else
                {
                    return null;
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, string.Join(string.Empty, chars));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<char> ParseSimpleSingleQuotedCharacter(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 =
                this.ParseLiteral("\'", cursor) ??
                this.ParseLiteral("\\", cursor) ??
                this.ParseEolChar(cursor);
            if (l1 != null)
            {
                return null;
            }

            var any1 = this.ParseAny(cursor);
            if (any1 != null)
            {
                cursor = cursor.Advance(any1);

                var len = cursor - startCursor;
                return new ParseResult<char>(len, any1.Value[0]);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<char> ParseSimpleDoubleQuotedCharacter(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 =
                this.ParseLiteral("\"", cursor) ??
                this.ParseLiteral("\\", cursor) ??
                this.ParseEolChar(cursor);
            if (l1 != null)
            {
                return null;
            }

            var any1 = this.ParseAny(cursor);
            if (any1 != null)
            {
                cursor = cursor.Advance(any1);

                var len = cursor - startCursor;
                return new ParseResult<char>(len, any1.Value[0]);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseEolChar(Cursor cursor)
        {
            return this.ParseRegex("[\n\r\u2028\u2029]", cursor);
        }

        private ParseResult<char> ParseSimpleEscapeSequence(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("\\", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var l2 =
                    this.ParseDigit(cursor) ??
                    this.ParseLiteral("x", cursor) ??
                    this.ParseLiteral("u", cursor) ??
                    this.ParseEolChar(cursor);
                if (l2 != null)
                {
                    return null;
                }

                var any1 = this.ParseAny(cursor);
                if (any1 != null)
                {
                    cursor = cursor.Advance(any1);

                    var len = cursor - startCursor;
                    return new ParseResult<char>(len, any1.Value
                        .Replace("b", "\b")
                        .Replace("f", "\f")
                        .Replace("n", "\n")
                        .Replace("r", "\r")
                        .Replace("t", "\t")
                        .Replace("v", "\v")[0]);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<char> ParseZeroEscapeSequence(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("\\", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var l2 = this.ParseLiteral("0", cursor);
                if (l2 != null)
                {
                    cursor = cursor.Advance(l2);

                    var digit1 = this.ParseDigit(cursor);
                    if (digit1 != null)
                    {
                        return null;
                    }

                    var len = cursor - startCursor;
                    return new ParseResult<char>(len, (char)0);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<char> ParseHexEscapeSequence(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("\\x", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var hexDigit1 = this.ParseHexDigit(cursor);
                if (hexDigit1 != null)
                {
                    cursor = cursor.Advance(hexDigit1);

                    var hexDigit2 = this.ParseHexDigit(cursor);
                    if (hexDigit2 != null)
                    {
                        cursor = cursor.Advance(hexDigit2);

                        var len = cursor - startCursor;
                        return new ParseResult<char>(len, (char)Convert.ToInt32(hexDigit1.Value + hexDigit2.Value, 16));
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<char> ParseUnicodeEscapeSequence(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("\\u", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var hexDigit1 = this.ParseHexDigit(cursor);
                if (hexDigit1 != null)
                {
                    cursor = cursor.Advance(hexDigit1);

                    var hexDigit2 = this.ParseHexDigit(cursor);
                    if (hexDigit2 != null)
                    {
                        cursor = cursor.Advance(hexDigit2);

                        var hexDigit3 = this.ParseHexDigit(cursor);
                        if (hexDigit3 != null)
                        {
                            cursor = cursor.Advance(hexDigit3);

                            var hexDigit4 = this.ParseHexDigit(cursor);
                            if (hexDigit4 != null)
                            {
                                cursor = cursor.Advance(hexDigit4);

                                var len = cursor - startCursor;
                                return new ParseResult<char>(len, (char)Convert.ToInt32(hexDigit1.Value + hexDigit2.Value + hexDigit3.Value + hexDigit4.Value, 16));
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseHexDigit(Cursor cursor)
        {
            return this.ParseRegex("[0-9a-fA-F]", cursor);
        }

        private ParseResult<char> ParseEolEscapeSequence(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("\\x", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var eol1 = this.ParseEol(cursor);
                if (eol1 != null)
                {
                    cursor = cursor.Advance(eol1);

                    var len = cursor - startCursor;
                    return new ParseResult<char>(len, '\n');
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseEquals(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("=", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseColon(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral(":", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseSemicolon(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral(";", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseSlash(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("/", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseAnd(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("&", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseNot(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("!", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseQuestion(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("?", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseStar(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("*", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParsePlus(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("+", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseLParen(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("(", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseRParen(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral(")", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseDot(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral(".", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseLt(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("<", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseGt(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral(">", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                var ws1 = this.ParseWs(cursor);
                if (ws1 != null)
                {
                    cursor = cursor.Advance(ws1);
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, l1.Value);
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseIdentifier(Cursor cursor)
        {
            var startCursor = cursor;

            var digit1 = this.ParseDigit(cursor);
            if (digit1 != null)
            {
                return null;
            }

            var name = new List<string>();
            while (true)
            {
                var l1 =
                    this.ParseLetter(cursor) ??
                    this.ParseDigit(cursor) ??
                    this.ParseLiteral("_", cursor) ??
                    this.ParseLiteral("$", cursor);

                if (l1 != null)
                {
                    cursor = cursor.Advance(l1);
                    name.Add(l1.Value);
                }
                else
                {
                    break;
                }
            }

            if (name.Count == 0)
            {
                return null;
            }

            var ws1 = this.ParseWs(cursor);
            if (ws1 != null)
            {
                cursor = cursor.Advance(ws1);
            }

            var len = cursor - startCursor;
            return new ParseResult<string>(len, string.Join(string.Empty, name));
        }

        private ParseResult<string> ParseDotted(Cursor cursor)
        {
            var startCursor = cursor;

            var segment1 = this.ParseSegment(cursor);
            if (segment1 != null)
            {
                cursor = cursor.Advance(segment1);

                while (true)
                {
                    var dot1 = this.ParseDot(cursor);
                    if (dot1 != null)
                    {
                        cursor = cursor.Advance(dot1);

                        var segment2 = this.ParseSegment(cursor);
                        if (segment2 != null)
                        {
                            cursor = cursor.Advance(segment2);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, cursor.Subject.Substring(startCursor.Location, len));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseSegment(Cursor cursor)
        {
            var startCursor = cursor;

            var identifier1 = this.ParseIdentifier(cursor);
            if (identifier1 != null)
            {
                cursor = cursor.Advance(identifier1);

                var startCursor1 = cursor;
                var lt1 = this.ParseLt(cursor);
                if (lt1 != null)
                {
                    cursor = cursor.Advance(lt1);

                    var dotted1 = this.ParseDotted(cursor);
                    if (dotted1 != null)
                    {
                        cursor = cursor.Advance(dotted1);

                        var gt1 = this.ParseGt(cursor);
                        if (gt1 != null)
                        {
                            cursor = cursor.Advance(gt1);
                        }
                        else
                        {
                            cursor = startCursor1;
                        }
                    }
                    else
                    {
                        cursor = startCursor1;
                    }
                }
                else
                {
                    cursor = startCursor1;
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, cursor.Subject.Substring(startCursor.Location, len));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseDigit(Cursor cursor)
        {
            return this.ParseRegex("[0-9]", cursor);
        }

        private ParseResult<string> ParseLetter(Cursor cursor)
        {
            return
                this.ParseLowerCaseLetter(cursor) ??
                this.ParseUpperCaseLetter(cursor);
        }

        private ParseResult<string> ParseUpperCaseLetter(Cursor cursor)
        {
            return this.ParseRegex("[A-Z]", cursor);
        }

        private ParseResult<string> ParseLowerCaseLetter(Cursor cursor)
        {
            return this.ParseRegex("[a-z]", cursor);
        }

        private ParseResult<string> ParseWs(Cursor cursor)
        {
            var startCursor = cursor;

            while (true)
            {
                var whitespace1 = this.ParseWhitespace(cursor);
                if (whitespace1 != null)
                {
                    cursor = cursor.Advance(whitespace1);
                }
                else
                {
                    var eol1 = this.ParseEol(cursor);
                    if (eol1 != null)
                    {
                        cursor = cursor.Advance(eol1);
                    }
                    else
                    {
                        var comment1 = this.ParseComment(cursor);
                        if (comment1 != null)
                        {
                            cursor = cursor.Advance(comment1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            var len = cursor - startCursor;
            return new ParseResult<string>(len, cursor.Subject.Substring(startCursor.Location, len));
        }

        private ParseResult<string> ParseComment(Cursor cursor)
        {
            return
                this.ParseSingleLineComment(cursor) ??
                this.ParseMultiLineComment(cursor);
        }

        private ParseResult<string> ParseSingleLineComment(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("//", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                while (true)
                {
                    var eolChar1 = this.ParseEolChar(cursor);
                    if (eolChar1 != null)
                    {
                        break;
                    }

                    var any1 = this.ParseAny(cursor);
                    if (any1 != null)
                    {
                        cursor = cursor.Advance(any1);
                    }
                    else
                    {
                        break;
                    }
                }

                var len = cursor - startCursor;
                return new ParseResult<string>(len, cursor.Subject.Substring(startCursor.Location, len));
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseMultiLineComment(Cursor cursor)
        {
            var startCursor = cursor;

            var l1 = this.ParseLiteral("/*", cursor);
            if (l1 != null)
            {
                cursor = cursor.Advance(l1);

                while (true)
                {
                    var l2 = this.ParseLiteral("*/", cursor);
                    if (l2 != null)
                    {
                        break;
                    }

                    var any1 = this.ParseAny(cursor);
                    if (any1 != null)
                    {
                        cursor = cursor.Advance(any1);
                    }
                    else
                    {
                        break;
                    }
                }

                var l3 = this.ParseLiteral("*/", cursor);
                if (l3 != null)
                {
                    cursor = cursor.Advance(l3);

                    var len = cursor - startCursor;
                    return new ParseResult<string>(len, cursor.Subject.Substring(startCursor.Location, len));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private ParseResult<string> ParseEol(Cursor cursor)
        {
            return
                this.ParseLiteral("\n", cursor) ??
                this.ParseLiteral("\r\n,", cursor) ??
                this.ParseLiteral("\r", cursor) ??
                this.ParseLiteral("\u2028", cursor) ??
                this.ParseLiteral("\u2029", cursor);
        }

        private ParseResult<string> ParseWhitespace(Cursor cursor)
        {
            return this.ParseRegex("[ \t\v\f\u00A0\uFEFF\u1680\u180E\u2000-\u200A\u202F\u205F\u3000]", cursor);
        }

        private ParseResult<string> ParseAny(Cursor cursor)
        {
            if (cursor.Location < cursor.Subject.Length)
            {
                var substr = cursor.Subject.Substring(cursor.Location, 1);
                return new ParseResult<string>(1, substr);
            }

            return null;
        }

        private ParseResult<string> ParseLiteral(string literal, Cursor cursor)
        {
            if (cursor.Location + literal.Length <= cursor.Subject.Length)
            {
                var substr = cursor.Subject.Substring(cursor.Location, literal.Length);
                if (substr == literal)
                {
                    return new ParseResult<string>(substr.Length, substr);
                }
            }

            return null;
        }

        private ParseResult<string> ParseRegex(string pattern, Cursor cursor)
        {
            var regex = new Regex(pattern, RegexOptions.Compiled);
            var match = regex.Match(cursor.Subject, cursor.Location);
            if (match.Success && match.Index == cursor.Location)
            {
                return new ParseResult<string>(match.Length, match.Value);
            }
            else
            {
                return null;
            }
        }
    }
}
