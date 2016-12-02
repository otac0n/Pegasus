---
layout: default
---
### Features ###

Pegasus is a PEG (Parsing Expression Grammar) parser generator for .NET that integrates with MSBuild and Visual Studio.

* Support for everything in the [formal definition of PEG](http://www.brynosaurus.com/pub/lang/peg-slides.pdf).
* Integration with the Visual Studio / MSBuild build pipeline.
* Limited debugging support (code sections only) in Visual Studio.
* Syntax highlighting with the [Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=JohnGietzen.Pegasus)
* Packrat parsing via the `-memoize` rule flag.
* Stateful parsing, including backtracking of state.

### Syntax Examples ###

Mathematical Expression Evaluator

    @namespace PegExamples
    @classname MathExpressionParser

    start <decimal>
      = _ value:additive _ EOF { value }

    additive <decimal> -memoize
        = left:additive _ "+" _ right:multiplicative { left + right }
        / left:additive _ "-" _ right:multiplicative { left - right }
        / multiplicative

    multiplicative <decimal> -memoize
        = left:multiplicative _ "*" _ right:primary { left * right }
        / left:multiplicative _ "/" _ right:primary { left / right }
        / primary

    primary <decimal>
        = decimal
        / "(" _ additive:additive _ ")" { additive }

    decimal <decimal>
        = value:([0-9]+ ("." [0-9]+)?) { decimal.Parse(value) }

    _ = [ \t\r\n]*

    EOF
      = !.
      / unexpected:. #ERROR{ "Unexpected character '" + unexpected + "'." }
