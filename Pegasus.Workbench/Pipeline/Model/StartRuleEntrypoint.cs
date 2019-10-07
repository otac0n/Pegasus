// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Workbench.Pipeline.Model
{
    using Pegasus.Common.Tracing;
    using Pegasus.Expressions;

    /// <summary>
    /// Represents a starting rule, exposed via that <c>Parse</c> method.
    /// </summary>
    public sealed class StartRuleEntrypoint : ParserEntrypoint
    {
        private readonly object parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartRuleEntrypoint"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="startRule">The starting rule.</param>
        public StartRuleEntrypoint(object parser, Rule startRule)
            : base("Parse", startRule)
        {
            this.parser = parser;
        }

        /// <inheritdoc />
        public override object Parse(string subject, string filename, ITracer tracer = null)
        {
            if (this.parser == null)
            {
                return null;
            }

            var type = this.parser.GetType();
            type.GetProperty("Tracer").SetValue(this.parser, tracer);
            return type.GetMethod("Parse").Invoke(this.parser, new object[] { subject, filename });
        }
    }
}
