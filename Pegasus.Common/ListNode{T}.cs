// -----------------------------------------------------------------------
// <copyright file="ListNode{T}.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Common
{
    /// <summary>
    /// Represents a node in a read-only list of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class ListNode<T>
    {
        private readonly T head;
        private readonly ListNode<T> tail;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListNode&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="head">The head of the list.</param>
        /// <param name="tail">The tail of the list.</param>
        public ListNode(T head, ListNode<T> tail = null)
        {
            this.head = head;
            this.tail = tail;
        }

        /// <summary>
        /// Gets the head of the list.
        /// </summary>
        public T Head
        {
            get { return this.head; }
        }

        /// <summary>
        /// Gets the tail of the list.
        /// </summary>
        public ListNode<T> Tail
        {
            get { return this.tail; }
        }
    }
}
