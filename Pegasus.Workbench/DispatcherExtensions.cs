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
