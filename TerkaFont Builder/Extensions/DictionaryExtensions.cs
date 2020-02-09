namespace Terka.FontBuilder.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains extension of <see cref="IDictionary{TKey,TValue}"/> interface.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets a value from the dictionary (or default value of the value type if the dictionary doesn't contain given key).
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>Value from the dictionary.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.ContainsKey(key) ? dict[key] : default(TValue);
        }

        /// <summary>
        /// Adds the specified value to the multi value dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TList">The type of the list.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="thisDictionary">The this dictionary.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public static void AddListMember<TKey, TList, TValue>(this Dictionary<TKey, TList> thisDictionary,
                                                             TKey key, TValue value)
        where TList : IList<TValue>, new()
        {
            //if the dictionary doesn't contain the key, make a new list under the key
            if (!thisDictionary.ContainsKey(key))
            {
                thisDictionary.Add(key, new TList());
            }

            //add the value to the list at the key index
            thisDictionary[key].Add(value);
        }
    }
}
