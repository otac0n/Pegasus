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
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportSettingsIssuesPass : CompilePass
    {
        private static readonly Dictionary<string, bool> KnownSettings = new Dictionary<string, bool>
        {
            { "namespace", true },
            { "classname", true },
            { "using", false },
        };

        public override void Run(Grammar grammar, CompileResult result)
        {
            var seenSettings = new HashSet<string>();

            foreach (var setting in grammar.Settings)
            {
                bool singleAllowed;
                if (KnownSettings.TryGetValue(setting.Key, out singleAllowed))
                {
                    if (singleAllowed && !seenSettings.Add(setting.Key))
                    {
                        result.Errors.Add(
                            new CompilerError(string.Empty, 0, 0, "PEG0005", string.Format(Resources.PEG0005_SETTING_ALREADY_SPECIFIED, setting.Key)));
                    }
                }
                else
                {
                    result.Errors.Add(
                        new CompilerError(string.Empty, 0, 0, "PEG0006", string.Format(Resources.PEG0006_SETTING_UNKNOWN, setting.Key)) { IsWarning = true });
                }
            }
        }
    }
}
