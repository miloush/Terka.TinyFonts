namespace Terka.FontBuilder.Optimizer
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A queue which can quickly retrieve first item from the queue and all items which are equal to it.
    /// </summary>
    /// <typeparam name="T">Type of items in the queue.</typeparam>
    public class FastEquivalenceQueue<T>
    {
        /// <summary>
        /// Internal representation of the queue. It only contains the first from each equivalence group.
        /// </summary>
        private readonly Queue<T> queue = new Queue<T>();

        /// <summary>
        /// Contains the same items as <see cref="queue"/>. Used to provide fast access to equivalence class of a particular item.
        /// </summary>
        private readonly Dictionary<int, List<T>> multiSet = new Dictionary<int, List<T>>();

        /// <summary>
        /// Comparer used to compare the items and generate hash codes.
        /// </summary>
        private readonly IEqualityComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastEquivalenceQueue{T}"/> class.
        /// </summary>
        /// <param name="comparer">Comparer used to compare the items and generate hash codes.</param>
        public FastEquivalenceQueue(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// Gets a value indicating whether this queue is empty.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty
        {
            get
            {
                return this.queue.Count == 0;
            }
        }

        /// <summary>
        /// Gets the number of items in the queue (among all the equivalency classes).
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get
            {
                return this.multiSet.Sum(p => p.Value.Count);
            }
        }

        /// <summary>
        /// Adds a new item to the end of the queue.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Enqueue(T item)
        {
            var hashCode = this.comparer.GetHashCode(item);
            if (this.multiSet.ContainsKey(hashCode))
            {
                // TODO: This is likely not very efficient!
                var equivalencySet = this.multiSet[hashCode];
                if (!equivalencySet.Contains(item))
                {
                    equivalencySet.Add(item);    
                }                
            }
            else
            {
                this.queue.Enqueue(item);
                var list = new List<T> { item };
                this.multiSet[hashCode] = list;
            }
        }

        /// <summary>
        /// Removes first item from the queue and returns it.
        /// </summary>
        /// <returns>First item from the queue.</returns>
        public IEnumerable<T> DequeueEquivalenceSet()
        {
            var item = this.queue.Dequeue();
            int hashCode = this.comparer.GetHashCode(item); 
            var multiSetList = this.multiSet[hashCode];

            // This is optimized for the case when there is no hash code collision
            var removedItems = multiSetList.Where(currentItem => !this.comparer.Equals(item, currentItem)).ToList();
            if (removedItems.Count == 0)
            {
                this.multiSet.Remove(hashCode);
            }
            else
            {
                this.multiSet[hashCode] = removedItems;
                this.queue.Enqueue(removedItems.First()); // Again, this is optimized for no-colission case (the conflicting items will end up being kicked to end of the queue)
                multiSetList = multiSetList.Except(removedItems).ToList();
            }

            return multiSetList;
        }
    }
}
