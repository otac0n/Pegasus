// -----------------------------------------------------------------------
// <copyright file="ReportSettingsIssuesPass.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportSettingsIssuesPass : CompilePass
    {
        private static readonly Dictionary<string, bool> KnownSettings = new Dictionary<string, bool>
        {
            { "namespace", true },
            { "classname", true },
            { "accessibility", true },
            { "resources", true },
            { "start", true },
            { "members", false },
            { "using", false },
        };

        private static readonly Dictionary<string, string> ValuePatterns = new Dictionary<string, string>
        {
            { "accessibility", @"^\s*(public|internal)\s*$" },
        };

        public override IList<string> BlockedByErrors
        {
            get { return new string[0]; }
        }

        public override IList<string> ErrorsProduced
        {
            get { return new[] { "PEG0005", "PEG0012" }; }
        }

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
                        result.AddCompilerError(cursor, () => Resources.PEG0005_ERROR_SettingAlreadySpecified, setting.Key.Name);
                    }
                }
                else
                {
                    result.AddCompilerError(cursor, () => Resources.PEG0006_WARNING_SettingUnknown, setting.Key.Name);
                }

                string pattern;
                if (ValuePatterns.TryGetValue(setting.Key.Name, out pattern))
                {
                    if (!Regex.IsMatch(setting.Value.ToString(), pattern))
                    {
                        result.AddCompilerError(cursor, () => Resources.PEG0012_ERROR_SettingValueInvalid, setting.Value, setting.Key.Name);
                    }
                }
            }
        }
    }
}
