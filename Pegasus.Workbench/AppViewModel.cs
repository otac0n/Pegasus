// -----------------------------------------------------------------------
// <copyright file="AppViewModel.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using ReactiveUI;

    public class AppViewModel : ReactiveObject
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { this.RaiseAndSetIfChanged(ref text, value); }
        }
    }
}
