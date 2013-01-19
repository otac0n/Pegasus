---
layout: default
---
### Features ###

Pegasus is a PEG (Parsing Expression Grammar) parser generator for .NET that integrates with MSBuild and Visual Studio.

* Support for everything in the [formal definition of PEG](http://www.brynosaurus.com/pub/lang/peg-slides.pdf).
* Integration with the Visual Studio / MSBuild build pipeline.
* Limited debugging support (code sections only) in Visual Studio.
* Packrat parsing via the `-memoize` rule flag.
* Stateful parsing, including backtracking of state.

### Syntax Examples ###

Mathematical Expression Evaluator

    @namespace PegExamples
    @classname MathExpressionParser

    start
      = value:additive EOF { value }

    additive <decimal>
      = left:multiplicative add right:additive { left + right }
      / left:multiplicative sub right:additive { left - right }
      / multiplicative

    multiplicative <decimal> -memoize
      = left:primary mul right:multiplicative { left * right }
      / left:primary div right:multiplicative { left / right }
      / primary

    primary <decimal> -memoize
      = decimal
      / lParen additive:additive rParen { additive }

    add =    "+" __ { "+" }
    sub =    "-" __ { "-" }
    mul =    "*" __ { "*" }
    div =    "/" __ { "/" }
    lParen = "(" __ { "(" }
    rParen = ")" __ { ")" }

    decimal <decimal>
      = value:([0-9]+ ("." [0-9]+)?) __ { decimal.Parse(value) }

    __ = [ \t\r\n]*

    EOF
      = !.
      / unexpected:. #ERROR{ "Unexpected character '" + unexpected + "'." }
