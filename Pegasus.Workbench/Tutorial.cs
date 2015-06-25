// -----------------------------------------------------------------------
// <copyright file="Tutorial.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Implements loading of tutorials.
    /// </summary>
    public class Tutorial
    {
        private static IReadOnlyList<Tutorial> allTutorials;

        private readonly string fileName;
        private readonly string grammar;
        private readonly string name;
        private readonly string testText;

        private Tutorial(string name, string grammar, string testText, string fileName)
        {
            this.name = name;
            this.grammar = grammar;
            this.testText = testText;
            this.fileName = fileName;
        }

        /// <summary>
        /// Gets the filename of this tutorial.
        /// </summary>
        /// <remarks>This file may not exist.</remarks>
        public string FileName
        {
            get { return this.fileName; }
        }

        /// <summary>
        /// Gets the grammar text for this tutorial.
        /// </summary>
        public string GrammarText
        {
            get { return this.grammar; }
        }

        /// <summary>
        /// Gets the name of this tutorial.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the test text for this tutorial.
        /// </summary>
        public string TestText
        {
            get { return this.testText; }
        }

        /// <summary>
        /// Loads all tutorials found in the assembly.
        /// </summary>
        /// <returns>All tutorials that were found.</returns>
        public static IReadOnlyList<Tutorial> FindAll()
        {
            return LazyInitializer.EnsureInitialized(ref allTutorials, () =>
            {
                var assembly = Assembly.GetExecutingAssembly();

                var resourcePrefix = assembly.GetName().Name + ".Tutorials.";
                var path = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(assembly.CodeBase).Path)), "Tutorials");

                var load = new Func<string, string>(name =>
                {
                    using (var stream = assembly.GetManifestResourceStream(resourcePrefix + name))
                    using (var reader = new StreamReader(stream, Encoding.Default, true, 1024, true))
                    {
                        return reader.ReadToEnd();
                    }
                });

                var resources = assembly.GetManifestResourceNames();
                var tutorialFiles = (from r in resources
                                     where r.StartsWith(resourcePrefix, StringComparison.Ordinal)
                                     select r.Substring(resourcePrefix.Length)).ToLookup(r => Path.GetExtension(r));
                return (from peg in tutorialFiles[".peg"]
                        let name = Path.GetFileNameWithoutExtension(peg)
                        join txt in tutorialFiles[".txt"]
                        on name equals Path.GetFileNameWithoutExtension(txt)
                        select new Tutorial(name, load(peg), load(txt), Path.Combine(path, peg))).ToList().AsReadOnly();
            });
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.name;
        }
    }
}
