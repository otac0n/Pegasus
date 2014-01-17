// -----------------------------------------------------------------------
// <copyright file="DispatcherExtensions.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Workbench
{
    using System;
    using System.Windows.Threading;

    internal static class DispatcherExtensions
    {
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action)
        {
            return dispatcher.BeginInvoke(action, null);
        }
    }
}
