// -----------------------------------------------------------------------
// <copyright file="ListNode.cs" company="(none)">
//   Copyright © 2015 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Common
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides static methods for operating on <see cref="ListNode{T}"/> objects.
    /// </summary>
    public static class ListNode
    {
        /// <summary>
        /// Prepends a element to the given read-only  <see cref="ListNode{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="this">The list being prepended.</param>
        /// <param name="value">The value to prepend to the list.</param>
        /// <returns>A new read-only list with the given value prepended.</returns>
        public static ListNode<T> Push<T>(this ListNode<T> @this, T value)
        {
            return new ListNode<T>(value, @this);
        }

        /// <summary>
        /// Converts a read-only <see cref="ListNode{T}"/> into a <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="this">The list to convert.</param>
        /// <returns>A <see cref="List{T}"/> containing the elements in the read-only <see cref="ListNode{T}"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "This does not expose an internal list, it is a utility method explicitly for the purpose of creating generic lists.")]
        public static List<T> ToList<T>(this ListNode<T> @this)
        {
            var list = new List<T>();

            while (@this != null)
            {
                list.Add(@this.Head);
                @this = @this.Tail;
            }

            return list;
        }
    }
}
