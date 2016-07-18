// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Workbench
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;
    using Pegasus.Workbench.Properties;

    /// <summary>
    /// Contains commands that are available to the application.
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// Gets the "LoadTutorial" command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This is the intended usage of the RoutedUICommand class.")]
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is the intended usage of the RoutedUICommand class.")]
        public static readonly RoutedUICommand LoadTutorial = new RoutedUICommand(Resources.LoadTutorial, nameof(LoadTutorial), typeof(Commands));
    }
}
