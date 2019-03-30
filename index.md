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

### Syntax Example ###

Mathematical Expression Evaluator

    @namespace PegExamples
    @classname MathExpressionParser

    start <double>
      = _ value:additive _ EOF { value }

    additive <double> -memoize
        = left:additive _ "+" _ right:multiplicative { left + right }
        / left:additive _ "-" _ right:multiplicative { left - right }
        / multiplicative

    multiplicative <double> -memoize
        = left:multiplicative _ "*" _ right:power { left * right }
        / left:multiplicative _ "/" _ right:power { left / right }
        / power

    power <double>
        = left:primary _ "^" _ right:power { Math.Pow(left, right) }
        / primary

    primary <double> -memoize
        = decimal
        / "-" _ primary:primary { -primary }
        / "(" _ additive:additive _ ")" { additive }

    decimal <double>
        = value:([0-9]+ ("." [0-9]+)?) { double.Parse(value) }

    _ = [ \t\r\n]*

    EOF
      = !.
      / unexpected:. #error{ "Unexpected character '" + unexpected + "'." }
