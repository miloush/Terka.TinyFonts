namespace Terka.FontBuilder.Simulator.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains extensions for the <see cref="LinkedListNode{T}"/> class.
    /// </summary>
    public static class LinkedListNodeExtensions
    {
        /// <summary>
        /// Finds node by X nodes towards the end of the list.
        /// </summary>
        /// <typeparam name="T">Type of item stored in the node.</typeparam>
        /// <param name="node">The node.</param>
        /// <param name="by">How many nodes towards the ends of the list.</param>
        /// <returns>The node or null if end of the list is reached.</returns>
        public static LinkedListNode<T> NextBy<T>(this LinkedListNode<T> node, int by)
        {
            if (by < 0)
            {
                return node.PreviousBy(-by);
            }

            var currentNode = node;
            for (int i = 0; i < by; i++)
            {
                if (currentNode == null)
                {
                    break;
                }

                currentNode = currentNode.Next;
            }

            return currentNode;
        }

        /// <summary>
        /// Finds node by X nodes towards the beginning of the list.
        /// </summary>
        /// <typeparam name="T">Type of item stored in the node.</typeparam>
        /// <param name="node">The node.</param>
        /// <param name="by">How many nodes towards the ends of the list.</param>
        /// <returns>The node or null if beginning of the list is reached.</returns>
        public static LinkedListNode<T> PreviousBy<T>(this LinkedListNode<T> node, int by)
        {
            if (by < 0)
            {
                return node.NextBy(-by);
            }

            var currentNode = node;
            for (int i = 0; i < by; i++)
            {
                if (currentNode == null)
                {
                    break;
                }

                currentNode = currentNode.Previous;
            }

            return currentNode;
        }
    }
}
