// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Workbench.Pipeline.Model
{
    using System.Reflection;
    using Pegasus.Common.Tracing;
    using Pegasus.Compiler;
    using Pegasus.Expressions;

    public sealed class PublicRuleEntrypoint : ParserEntrypoint
    {
        private readonly MethodInfo methodInfo;
        private readonly object parser;

        public PublicRuleEntrypoint(object parser, Rule rule)
            : base("Parse" + PublicRuleFinder.GetPublicName(rule), rule)
        {
            this.parser = parser;
            this.methodInfo = parser?.GetType().GetMethod(this.Name);
        }

        public override object Parse(string subject, string filename, ITracer tracer = null)
        {
            if (this.parser == null)
            {
                return null;
            }

            this.parser.GetType().GetProperty("Tracer").SetValue(this.parser, tracer);
            return this.methodInfo.Invoke(this.parser, new object[] { subject, filename });
        }
    }
}
