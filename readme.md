Pegasus
=======

Pegasus is a PEG-style parser generator for C# that integrates with MSBuild and Visual Studio.

Getting Started
---------------

The easiest way to get a copy of Pegasus is to install the [Pegasus NuGet package](http://nuget.org/packages/Pegasus) in Visual Studio.

    PM> Install-Package Pegasus

Due to a limitation in Visual Studio, you will need to reload your project for the 'PegGrammar' build action to be recognized.

Once you have the package installed, files in your project marked as 'PegGrammar' in the properties window will be compiled to their respective `.peg.cs` parser classes before every build.  These parser classes will be automatically included in compilation.

For help with grammar syntax, see [the syntax guide wiki entry](https://github.com/otac0n/Pegasus/wiki/Syntax-Guide)

Example
-------

Here is an example of a simple parser for mathematical expressions:

    @namespace MyProject
    @classname ExpressionParser

    additive <decimal> -memoize
        = left:additive "+" right:multiplicative { left + right }
        / left:additive "-" right:multiplicative { left - right }
        / multiplicative

    multiplicative <decimal> -memoize
        = left:multiplicative "*" right:primary { left * right }
        / left:multiplicative "/" right:primary { left / right }
        / primary

    primary <decimal>
        = decimal
        / "(" additive:additive ")" { additive }

    decimal <decimal>
        = value:([0-9]+ ("." [0-9]+)?) { decimal.Parse(value) }

This will take mathematical expressions as strings and evaluate them with the proper order of operations and associativity to produce a result as a decimal.

The above parser would be used like so:

    var parser = new MyProject.ExpressionParser();
    var result = parser.Parse("5.1+2*3");
    Console.WriteLine(result); // Outputs "11.1".
