// -----------------------------------------------------------------------
// <copyright file="ReportSettingsIssuesPass.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportSettingsIssuesPass : CompilePass
    {
        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0006", "PEG0012" }; }
        }

        public override IList<string> BlockedByErrors
        {
            get { return new string[0]; }
        }

        private static readonly Dictionary<string, bool> KnownSettings = new Dictionary<string, bool>
        {
            { "namespace", true },
            { "classname", true },
            { "accessibility", true },
            { "members", false },
            { "using", false },
        };

        private static readonly Dictionary<string, string> ValuePatterns = new Dictionary<string, string>
        {
            { "accessibility", @"\s*(public|internal)\s*" },
        };

        public override void Run(Grammar grammar, CompileResult result)
        {
            var seenSettings = new HashSet<string>();

            foreach (var setting in grammar.Settings)
            {
                var cursor = setting.Key.Start;

                bool singleAllowed;
                if (KnownSettings.TryGetValue(setting.Key.Name, out singleAllowed))
                {
                    if (singleAllowed && !seenSettings.Add(setting.Key.Name))
                    {
                        result.Errors.Add(
                            new CompilerError(cursor.FileName, cursor.Line, cursor.Column, "PEG0005", string.Format(Resources.PEG0005_SETTING_ALREADY_SPECIFIED, setting.Key.Name)));
                    }
                }
                else
                {
                    result.Errors.Add(
                        new CompilerError(cursor.FileName, cursor.Line, cursor.Column, "PEG0006", string.Format(Resources.PEG0006_SETTING_UNKNOWN, setting.Key.Name)) { IsWarning = true });
                }

                string pattern;
                if (ValuePatterns.TryGetValue(setting.Key.Name, out pattern))
                {
                    if (!Regex.IsMatch(setting.Value, pattern))
                    {
                        result.Errors.Add(
                            new CompilerError(cursor.FileName, cursor.Line, cursor.Column, "PEG0012", string.Format(Resources.PEG0012_SETTING_VALUE_INVALID, setting.Value, setting.Key.Name)));
                    }
                }
            }
        }
    }
}
