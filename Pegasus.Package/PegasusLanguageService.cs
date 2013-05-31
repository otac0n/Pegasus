// -----------------------------------------------------------------------
// <copyright file="PegasusLanguageService.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Package
{
    using Microsoft.VisualStudio.Package;
    using Microsoft.VisualStudio.TextManager.Interop;

    /// <summary>
    /// Implements a <see cref="LanguageService"/> for Pegasus grammars.
    /// </summary>
    public class PegasusLanguageService : LanguageService
    {
        private static LanguagePreferences preferences = new LanguagePreferences();

        /// <summary>
        /// Gets the name of the language service.
        /// </summary>
        public override string Name
        {
            get { return "Pegasus"; }
        }

        /// <summary>
        /// Returns a list of file extension filters suitable for a Save As dialog box.
        /// </summary>
        /// <returns>If successful, returns a string containing the file extension filters; otherwise, returns an empty string.</returns>
        public override string GetFormatFilterList()
        {
            return "Pegasus Grammars (*.peg)|*.peg";
        }

        /// <summary>
        /// Returns a <see cref="LanguagePreferences"/> object for this language service.
        /// </summary>
        /// <returns>If successful, returns a <see cref="LanguagePreferences"/> object; otherwise, returns a null value.</returns>
        public override LanguagePreferences GetLanguagePreferences()
        {
            return preferences;
        }

        /// <summary>
        /// Returns a single instantiation of a parser.
        /// </summary>
        /// <param name="buffer">An <see cref="IVsTextLines"/> representing the lines of source to parse.</param>
        /// <returns>If successful, returns an <see cref="IScanner"/> object; otherwise, returns a null value.</returns>
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return new Scanner(buffer);
        }

        /// <summary>
        /// Parses the source based on the specified <see cref="ParseRequest"/> object.
        /// </summary>
        /// <param name="req">The <see cref="ParseRequest"/> describing how to parse the source file.</param>
        /// <returns>If successful, returns an <see cref="AuthoringScope"/> object; otherwise, returns a null value.</returns>
        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return null;
        }

        private class Scanner : IScanner
        {
            private IVsTextLines buffer;
            private string source;
            private int offset;

            public Scanner(IVsTextLines buffer)
            {
                this.buffer = buffer;
            }

            public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
            {
                return false;
            }

            public void SetSource(string source, int offset)
            {
                this.source = source;
                this.offset = offset;
            }
        }
    }
}
