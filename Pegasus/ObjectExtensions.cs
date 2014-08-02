// -----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus
{
    using System;

    internal static class ObjectExtensions
    {
        public static T As<T>(this object @this) where T : class
        {
            return @this as T;
        }

        public static TOut As<TIn, TOut>(this TIn @this, Func<TIn, TOut> map)
        {
            return map(@this);
        }
    }
}
