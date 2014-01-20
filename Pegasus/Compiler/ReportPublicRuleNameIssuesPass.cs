namespace Pegasus.Compiler
{
    using System.Collections.Generic;
    using System.Linq;
    using Pegasus.Expressions;
    using Pegasus.Properties;

    internal class ReportPublicRuleNameIssuesPass : CompilePass
    {
        public override IList<string> BlockedByErrors
        {
            get { return new string[0]; }
        }

        public override IList<string> ErrorsProduced
        {
            get { return new string[0]; }
        }

        public override void Run(Grammar grammar, CompileResult result)
        {
            foreach (var rule in grammar.Rules)
            {
                if (char.IsLower(rule.Identifier.Name[0]))
                {
                    if (rule.Flags.Any(f => f.Name == "public"))
                    {
                        result.AddWarning(rule.Identifier.Start, () => Resources.PEG0023_LOWERCASE_PUBLIC_RULE, rule.Identifier.Name);
                    }
                }
            }
        }
    }
}
