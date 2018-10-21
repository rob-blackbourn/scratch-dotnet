using System;
using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Common.Collections.Generic
{
    /// <summary>
    /// Extension methods for nested dictionaries with two keys.
    /// </summary>
    public static class TwoKeyDictionaryExtensions
    {
        /// <summary>
        /// Creates a dictionary of dictionaries.
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the source collection.</typeparam>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source collection from which the dictionary is built.</param>
        /// <param name="keySelector1">A selector function for the first key.</param>
        /// <param name="keySelector2">A selector function for the second key.</param>
        /// <param name="valueSelector">The value selector.</param>
        /// <returns>A two key dictionary.</returns>
        public static Dictionary<TKey1, IDictionary<TKey2, TValue>> ToDictionary<TSource, TKey1, TKey2, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey1> keySelector1,
            Func<TSource, TKey2> keySelector2,
            Func<TSource, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey1, IDictionary<TKey2, TValue>>();

            foreach (var item in source)
                dictionary.Add(keySelector1(item), keySelector2(item), valueSelector(item));

            return dictionary;
        }

        /// <summary>
        /// Adds a value to a two key dictionary.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictByKey1">The outer dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="value">The value to add.</param>
        public static void Add<TKey1, TKey2, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, TValue>> dictByKey1,
            TKey1 key1,
            TKey2 key2,
            TValue value)
        {
            IDictionary<TKey2, TValue> dictByKey2;
            if (!dictByKey1.TryGetValue(key1, out dictByKey2))
                dictByKey1.Add(key1, dictByKey2 = new Dictionary<TKey2, TValue>());

            dictByKey2.Add(key2, value);
        }

        /// <summary>
        /// Tries to get a value from the dictionary with the given keys.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictByKey1">The outer dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="value">The value to be set if the keys were found.</param>
        /// <returns>If the keys were found true, otherwise false.</returns>
        public static bool TryGetValue<TKey1, TKey2, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, TValue>> dictByKey1,
            TKey1 key1,
            TKey2 key2,
            out TValue value)
        {
            IDictionary<TKey2, TValue> dictByKey2;
            if (dictByKey1.TryGetValue(key1, out dictByKey2))
                return dictByKey2.TryGetValue(key2, out value);

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Returns true if the keys were found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictByKey1">The outer dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <returns>If the keys were found true, otherwise false.</returns>
        public static bool ContainsKey<TKey1, TKey2, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, TValue>> dictByKey1,
            TKey1 key1,
            TKey2 key2)
        {
            IDictionary<TKey2, TValue> dictByKey2;
            return dictByKey1.TryGetValue(key1, out dictByKey2) && dictByKey2.ContainsKey(key2);
        }

        /// <summary>
        /// Removes an item from a two key dictionary.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictByKey1">The outer dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <returns>If the item was removed true, otherwise false.</returns>
        public static bool Remove<TKey1, TKey2, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, TValue>> dictByKey1,
            TKey1 key1,
            TKey2 key2)
        {
            IDictionary<TKey2, TValue> dictByKey2;
            if (dictByKey1.TryGetValue(key1, out dictByKey2))
            {
                if (!dictByKey2.Remove(key2))
                    return false;
                if (dictByKey2.Count == 0)
                    dictByKey1.Remove(key1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a value from a two key dictionary.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictByKey1">The outer dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <returns>The value found with the keys</returns>
        /// <exception cref="KeyNotFoundException">Throw if the key was not found.</exception>
        public static TValue Get<TKey1, TKey2, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, TValue>> dictByKey1,
            TKey1 key1,
            TKey2 key2)
        {
            TValue value;
            IDictionary<TKey2, TValue> dictByKey2;
            if (dictByKey1.TryGetValue(key1, out dictByKey2) && dictByKey2.TryGetValue(key2, out value))
                return value;
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Sets the value in a two key dictionary.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictByKey1">The outer dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="value">The value to set.</param>
        public static void Set<TKey1, TKey2, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, TValue>> dictByKey1,
            TKey1 key1,
            TKey2 key2,
            TValue value)
        {
            IDictionary<TKey2, TValue> dictByKey2;
            if (!dictByKey1.TryGetValue(key1, out dictByKey2))
                dictByKey1.Add(key1, dictByKey2 = new Dictionary<TKey2, TValue>());
            dictByKey2[key2] = value;
        }

        /// <summary>
        /// Creates an enumeration of the values in the dictionary.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <returns>An enumeration of the values in the dictionary.</returns>
        public static IEnumerable<TValue> ToValues<TKey1, TKey2, TValue>(this IDictionary<TKey1, IDictionary<TKey2, TValue>> source)
        {
            return source.SelectMany(a => a.Value.Select(b => b.Value));
        }
    }
}
