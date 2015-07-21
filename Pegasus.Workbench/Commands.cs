// -----------------------------------------------------------------------
// <copyright file="Commands.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;

    /// <summary>
    /// Contains commands that are available to the application.
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// Gets the "LoadTutorial" command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This is the intended usage of the RoutedUICommand class.")]
        public static readonly RoutedUICommand LoadTutorial = new RoutedUICommand("Load Tutorial", "LoadTutorial", typeof(Commands));
    }
}
