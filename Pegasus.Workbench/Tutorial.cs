// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

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

        private Tutorial(string name, string grammarText, string testText, string fileName)
        {
            this.Name = name;
            this.GrammarText = grammarText;
            this.TestText = testText;
            this.FileName = fileName;
        }

        /// <summary>
        /// Gets the filename of this tutorial.
        /// </summary>
        /// <remarks>This file may not exist.</remarks>
        public string FileName { get; }

        /// <summary>
        /// Gets the grammar text for this tutorial.
        /// </summary>
        public string GrammarText { get; }

        /// <summary>
        /// Gets the name of this tutorial.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the test text for this tutorial.
        /// </summary>
        public string TestText { get; }

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

                string Load(string name)
                {
                    using (var stream = assembly.GetManifestResourceStream(resourcePrefix + name))
                    using (var reader = new StreamReader(stream, Encoding.Default, true, 1024, true))
                    {
                        return reader.ReadToEnd();
                    }
                }

                var resources = assembly.GetManifestResourceNames();
                var tutorialFiles = (from r in resources
                                     where r.StartsWith(resourcePrefix, StringComparison.Ordinal)
                                     select r.Substring(resourcePrefix.Length)).ToLookup(r => Path.GetExtension(r));
                return (from peg in tutorialFiles[".peg"]
                        let name = Path.GetFileNameWithoutExtension(peg)
                        join txt in tutorialFiles[".txt"]
                        on name equals Path.GetFileNameWithoutExtension(txt)
                        select new Tutorial(name, Load(peg), Load(txt), Path.Combine(path, peg))).ToList().AsReadOnly();
            });
        }

        /// <inheritdoc />
        public override string ToString() => this.Name;
    }
}
