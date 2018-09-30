// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

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
            { SettingName.Namespace, true },
            { SettingName.ClassName, true },
            { SettingName.Accessibility, true },
            { SettingName.Resources, true },
            { SettingName.Start, true },
            { SettingName.Members, false },
            { SettingName.Using, false },
            { SettingName.Trace, true },
            { SettingName.IgnoreCase, true },
        };

        private static readonly Dictionary<string, string> ValuePatterns = new Dictionary<string, string>
        {
            { SettingName.Accessibility, @"^\s*(public|internal)\s*$" },
            { SettingName.Trace, @"^\s*(true|false)\s*$" },
            { SettingName.IgnoreCase, @"^\s*(true|false)\s*$" },
        };

        public override IList<string> BlockedByErrors => new string[0];

        public override IList<string> ErrorsProduced => new[] { "PEG0005", "PEG0012" };

        public override void Run(Grammar grammar, CompileResult result)
        {
            var seenSettings = new HashSet<string>();

            foreach (var setting in grammar.Settings)
            {
                var cursor = setting.Key.Start;

                if (KnownSettings.TryGetValue(setting.Key.Name, out var singleAllowed))
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

                if (ValuePatterns.TryGetValue(setting.Key.Name, out var pattern))
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
