// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.Globalization;
    using System.Threading;

    internal static class CultureUtilities
    {
        public static void WithCulture(string culture, Action action)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            var currentThread = Thread.CurrentThread;
            var originalCulture = currentThread.CurrentCulture;
            var originalUICulture = currentThread.CurrentUICulture;
            try
            {
                currentThread.CurrentCulture = cultureInfo;
                currentThread.CurrentUICulture = cultureInfo;
                action();
            }
            finally
            {
                currentThread.CurrentUICulture = originalUICulture;
                currentThread.CurrentCulture = originalCulture;
            }
        }

        public static T WithCulture<T>(string culture, Func<T> action)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            var currentThread = Thread.CurrentThread;
            var originalCulture = currentThread.CurrentCulture;
            var originalUICulture = currentThread.CurrentUICulture;
            try
            {
                currentThread.CurrentCulture = cultureInfo;
                currentThread.CurrentUICulture = cultureInfo;
                return action();
            }
            finally
            {
                currentThread.CurrentUICulture = originalUICulture;
                currentThread.CurrentCulture = originalCulture;
            }
        }
    }
}
