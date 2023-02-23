namespace Terka.FontBuilder.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // ReSharper disable CompareNonConstrainedGenericWithNull

    /// <summary>
    /// Contains some additional LINQ tools.
    /// </summary>
    public static class EnumerableExtensions
    {
        ///// <summary>
        ///// Returns the collection with specified element appended to its end.
        ///// </summary>
        ///// <typeparam name="T">Type of elements in the collection.</typeparam>
        ///// <param name="collection">The collection.</param>
        ///// <param name="appendee">The appendee.</param>
        ///// <returns>The collection with specified element appended to its end.</returns>
        //public static IEnumerable<T> Append<T>(this IEnumerable<T> collection, T appendee)
        //{
        //    if (collection == null)
        //    {
        //        throw new ArgumentNullException("collection");
        //    }

        //    if (appendee == null)
        //    {
        //        throw new ArgumentNullException("appendee");
        //    }

        //    foreach (var item in collection)
        //    {
        //        yield return item;
        //    }

        //    yield return appendee;
        //}

        /// <summary>
        /// Returns the collection with specified collection appended to its end.
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="appendee">The appendee.</param>
        /// <returns>The collection with specified element appended to its end.</returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> collection, IEnumerable<T> appendee)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (appendee == null)
            {
                throw new ArgumentNullException("appendee");
            }

            foreach (var item in collection)
            {
                yield return item;
            }

            foreach (var item in appendee)
            {
                yield return item;
            }
        }

        ///// <summary>
        ///// Returns the collection with specified element prepended before its first element.
        ///// </summary>
        ///// <typeparam name="T">Type of elements in the collection.</typeparam>
        ///// <param name="collection">The collection.</param>
        ///// <param name="prependee">The prependee.</param>
        ///// <returns>The collection with specified element prepended before its first element.</returns>
        //public static IEnumerable<T> Prepend<T>(this IEnumerable<T> collection, T prependee)
        //{
        //    if (collection == null)
        //    {
        //        throw new ArgumentNullException("collection");
        //    }

        //    if (prependee == null)
        //    {
        //        throw new ArgumentNullException("prependee");
        //    }

        //    yield return prependee;

        //    foreach (var item in collection)
        //    {
        //        yield return item;
        //    }
        //}

        /// <summary>
        /// Compares two collections.
        /// </summary>
        /// <typeparam name="T">Type of items in the collections.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="otherCollection">The other collection.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>True if all items in the first collection are equal to corresponding items in the other collection, false otherwise (including case when
        /// the collections have different lengths).</returns>
        public static bool ValuesEqual<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection, IEqualityComparer<T> comparer = null)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (otherCollection == null)
            {
                throw new ArgumentNullException("otherCollection");
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            var collectionAList = collection.ToList();
            var collectionBList = otherCollection.ToList();

            if (collectionAList.Count != collectionBList.Count)
            {
                return false;
            }

            return collectionAList.Zip(collectionBList, (a, b) => comparer.Equals(a, b)).All(p => p);
        }

#if !NET
        /// <summary>
        /// Zips the two collections together generating a collection of tuples.
        /// </summary>
        /// <typeparam name="T1">The type of the first collection.</typeparam>
        /// <typeparam name="T2">The type of the second collection.</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <returns>The zipped collection.</returns>
        public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
        {
            return first.Zip(second, Tuple.Create);
        }
#endif

        /// <summary>
        /// Converts a collection of key-value pairs to a dictionary.
        /// </summary>
        /// <typeparam name="T1">The type of the keys.</typeparam>
        /// <typeparam name="T2">The type of the values.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>The dictionary.</returns>
        public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> collection)
        {
            return collection.ToDictionary(p => p.Key, p => p.Value);
        }
    }
}
