// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Common
{
    /// <summary>
    /// Represents a node in a read-only list of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class ListNode<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListNode{T}"/> class.
        /// </summary>
        /// <param name="head">The head of the list.</param>
        /// <param name="tail">The tail of the list.</param>
        public ListNode(T head, ListNode<T> tail = null)
        {
            this.Head = head;
            this.Tail = tail;
        }

        /// <summary>
        /// Gets the head of the list.
        /// </summary>
        public T Head { get; }

        /// <summary>
        /// Gets the tail of the list.
        /// </summary>
        public ListNode<T> Tail { get; }
    }
}
