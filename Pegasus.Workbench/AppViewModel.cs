// -----------------------------------------------------------------------
// <copyright file="AppViewModel.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using ReactiveUI;

    public class AppViewModel : ReactiveObject
    {
        private IList<CompilerError> errors = new CompilerError[0];
        private string fileName = "";
        private string text;

        public AppViewModel()
        {
            this.WhenAny(x => x.Text, text =>
            {
                return CompileManager.CompileString(text.Value ?? "", this.fileName).Errors;
            }).BindTo(this, x => x.CompileErrors);
        }

        public IList<CompilerError> CompileErrors
        {
            get { return this.errors; }
            protected set { this.RaiseAndSetIfChanged(ref this.errors, value); }
        }

        public string FileName
        {
            get { return this.fileName; }
            set { this.RaiseAndSetIfChanged(ref this.fileName, value); }
        }

        public string Text
        {
            get { return this.text; }
            set { this.RaiseAndSetIfChanged(ref this.text, value); }
        }
    }
}
