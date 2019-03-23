// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Workbench.Pipeline.Model
{
    using Pegasus.Common.Tracing;
    using Pegasus.Expressions;

    /// <summary>
    /// The base class for parser entrypoints.
    /// </summary>
    public abstract class ParserEntrypoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserEntrypoint"/> class.
        /// </summary>
        /// <param name="name">The name of the entrypoint.</param>
        /// <param name="rule">The rule implementing the entrypoint.</param>
        public ParserEntrypoint(string name, Rule rule)
        {
            this.Name = name;
            this.Rule = rule;
        }

        /// <summary>
        /// Gets the name of the entrypoint.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the rule implementing the entrypoint.
        /// </summary>
        public Rule Rule { get; }

        /// <summary>
        /// Parse the specified subject.
        /// </summary>
        /// <param name="subject">The subject to parse.</param>
        /// <param name="filename">The filename of the subject.</param>
        /// <param name="tracer">The tracer to use.</param>
        /// <returns>The parse result.</returns>
        public abstract object Parse(string subject, string filename, ITracer tracer = null);
    }
}
