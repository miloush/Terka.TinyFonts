namespace Terka.TinyFonts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Collection with sentinel element.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class SentinelCollection<TItem> : IList<TItem>
    {
        private List<TItem> _items;

        private TItem _sentinel;
        /// <summary>
        /// Gets or sets sentinel element.
        /// </summary>
        public TItem Sentinel
        {
            get { return _sentinel; }
            set { _sentinel = value; }
        }

        /// <summary>
        /// Creates new instance of collection.
        /// </summary>
        public SentinelCollection()
        {
            _items = new List<TItem>();
        }
        /// <summary>
        /// Creates new instance of collection with custom <paramref name="sentinel"/>.
        /// </summary>
        /// <param name="sentinel">Sentinel element.</param>
        public SentinelCollection(TItem sentinel)
        {
            _sentinel = sentinel;
        }
        /// <summary>
        /// Creates new instance of collection from <paramref name="items"/> with custom <paramref name="sentinel"/>.
        /// </summary>
        /// <param name="items">Items to add to the collection.</param>
        /// <param name="sentinel">Sentinel element.</param>
        public SentinelCollection(IEnumerable<TItem> items, TItem sentinel)
        {
            if (items == null)
                throw new ArgumentNullException();

            _items = new List<TItem>(items);
            _sentinel = sentinel;
        }
        /// <summary>
        /// Creates new instance of collection from <paramref name="items"/> with optional sentinel as last element.
        /// </summary>
        /// <param name="items">Items to add to the collection.</param>
        /// <param name="endsWithSentinel">If true last element of <paramref name="items"/> will be handled as sentinel element.</param>
        public SentinelCollection(IEnumerable<TItem> items, bool endsWithSentinel) : this(items, default(TItem))
        {
            if (endsWithSentinel && _items.Count > 0)
            {
                _sentinel = _items[_items.Count - 1];
                _items.RemoveAt(_items.Count - 1);
            }
        }

        /// <summary>
        /// Gets count of elements in collection with sentinel.
        /// </summary>
        public int Count
        {
            get { return _items.Count + 1; }
        }
        /// <summary>
        /// Gets count of elements in collection without sentinel.
        /// </summary>
        public int ItemsCount
        {
            get { return _items.Count; }
        }
        /// <summary>
        /// Gets or sets collection capacity.
        /// </summary>
        public int Capacity
        {
            get { return _items.Capacity; }
            set { _items.Capacity = value; }
        }

        /// <summary>
        /// Adds new element to collection.
        /// </summary>
        /// <param name="item">Element to add.</param>
        public void Add(TItem item)
        {
            _items.Add(item);
        }
        /// <summary>
        /// Adds new items to collection.
        /// </summary>
        /// <param name="items">Items to add.</param>
        public void AddRange(IEnumerable<TItem> items)
        {
            _items.AddRange(items);
        }
        /// <summary>
        /// Inserts item to specified position.
        /// </summary>
        /// <param name="index">Position index in collection.</param>
        /// <param name="item">Item to add.</param>
        public void Insert(int index, TItem item)
        {
            if (index == _items.Count + 1)
                _sentinel = item;
            else
                _items.Insert(index, item); // throws on bad index
        }

        /// <summary>
        /// Tests if item exists in collection.
        /// </summary>
        /// <param name="item">Item to test.</param>
        /// <returns>True if item exists.</returns>
        public bool Contains(TItem item)
        {
            return IndexOf(item) >= 0;
        }
        /// <summary>
        /// Gets index of item in the collection.
        /// </summary>
        /// <param name="item">Item to search.</param>
        /// <returns>Zero-based index.</returns>
        public int IndexOf(TItem item)
        {
            int index = _items.IndexOf(item);
            if (index >= 0)
                return index;

            if (EqualityComparer<TItem>.Default.Equals(_sentinel, item))
                return _items.Count;

            return -1;
        }
        /// <summary>
        /// Gets if collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Binary search of item in the collection.
        /// </summary>
        /// <param name="item">Item to search</param>
        /// <returns>Zero-based index of item.</returns>
        public int BinarySearch(TItem item)
        {
            return _items.BinarySearch(item);
        }
        /// <summary>
        /// Binary search of item in the collection using custom comparer.
        /// </summary>
        /// <param name="item">Item to search</param>
        /// <param name="comparer">Custom comparer.</param>
        /// <returns>Zero-based index of item.</returns>
        public int BinarySearch(TItem item, IComparer<TItem> comparer)
        {
            return _items.BinarySearch(item, comparer);
        }
        /// <summary>
        /// Binary search of item in the collection using custom comparer.
        /// </summary>
        /// <param name="item">Item to search</param>
        /// <param name="comparer">Custom comparer.</param>
        /// <returns>Zero-based index of item.</returns>
        public int BinarySearch(object item, System.Collections.IComparer comparer)
        {
            return System.Collections.ArrayList.Adapter(_items).BinarySearch(item, comparer);
        }

        /// <summary>
        /// Sorts collection.
        /// </summary>
        public void Sort()
        {
            _items.Sort();
        }
        /// <summary>
        /// Sorts items in a range of items in collection using specified comparer.
        /// </summary>
        /// <param name="index">Starting index of range.</param>
        /// <param name="count">Count of items in range.</param>
        /// <param name="comparer">Custom comparer.</param>
        public void Sort(int index, int count, IComparer<TItem> comparer)
        {
            _items.Sort(index, count, comparer);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }
        /// <summary>
        /// Removes the first occurence of item in the collection.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>True if success.</returns>
        public bool Remove(TItem item)
        {
            return _items.Remove(item);
        }
        /// <summary>
        /// Removes item at specified index in the collection.
        /// </summary>
        /// <param name="index">Index of item to remove.</param>
        public void RemoveAt(int index)
        {
            if (index == _items.Count)
                _sentinel = default(TItem);
            else
                _items.RemoveAt(index);     // throws on bad index
        }

        /// <summary>
        /// Indexer to the collection.
        /// </summary>
        /// <param name="index">Items index.</param>
        /// <returns>Item.</returns>
        public TItem this[int index]
        {
            get
            {
                if (index == _items.Count)
                    return _sentinel;

                return _items[index];       // throws on bad index
            }
            set
            {
                if (index == _items.Count)
                    _sentinel = value;
                else
                    _items[index] = value;  // throws on bad index
            }
        }

        /// <summary>
        /// Copies entire collection to a compatible one-dimensional array starting at <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">Array to be filled.</param>
        /// <param name="arrayIndex">Starting index in array.</param>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (arrayIndex + _items.Count >= array.Length)
                throw new ArgumentException();

            _items.CopyTo(array, arrayIndex);
            array[arrayIndex + _items.Count] = _sentinel;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection with the sentinal.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return _items.Concat(new[] { _sentinel }).GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection with the sentinal.
        /// </summary>
        /// <returns>Enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("ItemsCount = {0}", ItemsCount);
        }
    }
}
