// -----------------------------------------------------------------------
// <copyright file="Commands.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System.Windows.Input;

    /// <summary>
    /// Contains commands that are available to the application.
    /// </summary>
    public static class Commands
    {
        private static readonly RoutedUICommand loadTutorial = new RoutedUICommand("Load Tutorial", "LoadTutorial", typeof(Commands));

        /// <summary>
        /// Get the "LoadTutorial" command.
        /// </summary>
        public static RoutedUICommand LoadTutorial { get { return loadTutorial; } }
    }
}
