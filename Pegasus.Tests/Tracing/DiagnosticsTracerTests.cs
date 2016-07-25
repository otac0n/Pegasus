// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Tracing
{
    using System;
    using System.Text.RegularExpressions;
    using Compiler;
    using NUnit.Framework;
    using Parser;
    using Pegasus.Common.Tracing;

    [TestFixture]
    public class DiagnosticsTracerTests
    {
        [Test]
        public void EndToEndTest()
        {
            var grammar = new PegParser().Parse("@trace true; start = basicRule leftRecursiveRule outerRule (memoizedRule 'NO' / memoizedRule)*; basicRule = 'OK'; leftRecursiveRule -memoize = leftRecursiveRule '+' 'OK' / 'OK'; memoizedRule -memoize = 'OK'; outerRule = innerRule; innerRule = 'OK';");
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<string>(compiled);

            parser.Tracer = DiagnosticsTracer.Instance;
            var output = TraceUtility.Trace(() =>
            {
                parser.Parse("OKOK+OKOKOK");
            });

            var stateKey = Regex.Match(output, @"state key (\d+)").Groups[1].Value;
            Assert.That(output, Is.EqualTo(StringUtilities.JoinLines(
                $"Begin 'start' at (1,1) with state key {stateKey}",
                $"    Begin 'basicRule' at (1,1) with state key {stateKey}",
                $"    End 'basicRule' with success at (1,3) with state key {stateKey}",
                $"    Begin 'leftRecursiveRule' at (1,3) with state key {stateKey}",
                $"        Cache miss.",
                $"        Seeding left-recursion with an unsuccessful match.",
                $"        Begin 'leftRecursiveRule' at (1,3) with state key {stateKey}",
                $"            Cache hit.",
                $"        End 'leftRecursiveRule' with failure at (1,3) with state key {stateKey}",
                $"        Caching result and retrying.",
                $"        Begin 'leftRecursiveRule' at (1,3) with state key {stateKey}",
                $"            Cache hit.",
                $"        End 'leftRecursiveRule' with success at (1,5) with state key {stateKey}",
                $"        Caching result and retrying.",
                $"        Begin 'leftRecursiveRule' at (1,3) with state key {stateKey}",
                $"            Cache hit.",
                $"        End 'leftRecursiveRule' with success at (1,8) with state key {stateKey}",
                $"        No forward progress made, current cache entry will be kept.",
                $"    End 'leftRecursiveRule' with success at (1,8) with state key {stateKey}",
                $"    Begin 'outerRule' at (1,8) with state key {stateKey}",
                $"        Begin 'innerRule' at (1,8) with state key {stateKey}",
                $"        End 'innerRule' with success at (1,10) with state key {stateKey}",
                $"    End 'outerRule' with success at (1,10) with state key {stateKey}",
                $"    Begin 'memoizedRule' at (1,10) with state key {stateKey}",
                $"        Cache miss.",
                $"        Caching result.",
                $"    End 'memoizedRule' with success at (1,12) with state key {stateKey}",
                $"    Begin 'memoizedRule' at (1,10) with state key {stateKey}",
                $"        Cache hit.",
                $"    End 'memoizedRule' with success at (1,12) with state key {stateKey}",
                $"    Begin 'memoizedRule' at (1,12) with state key {stateKey}",
                $"        Cache miss.",
                $"        Caching result.",
                $"    End 'memoizedRule' with failure at (1,12) with state key {stateKey}",
                $"    Begin 'memoizedRule' at (1,12) with state key {stateKey}",
                $"        Cache hit.",
                $"    End 'memoizedRule' with failure at (1,12) with state key {stateKey}",
                $"End 'start' with success at (1,12) with state key {stateKey}")));
        }
    }
}
