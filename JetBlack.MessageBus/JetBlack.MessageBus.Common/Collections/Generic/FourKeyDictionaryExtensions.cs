using System;
using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Common.Collections.Generic
{
    /// <summary>
    /// Extension methods for nested dictionaries for depth four.
    /// </summary>
    public static class FourKeyDictionaryExtensions
    {
        /// <summary>
        /// Converts an enumerable input into a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the source enumeration.</typeparam>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="source">The source enumeration.</param>
        /// <param name="keySelector1">A selector for the first key.</param>
        /// <param name="keySelector2">A selector for the second key.</param>
        /// <param name="keySelector3">A selector for the third key.</param>
        /// <param name="keySelector4">A selector for the fourth key.</param>
        /// <param name="valueSelector">The value selector.</param>
        /// <returns>A nested dictionary of depth four.</returns>
        public static Dictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> ToDictionary<TSource, TKey1, TKey2, TKey3, TKey4, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey1> keySelector1,
            Func<TSource, TKey2> keySelector2,
            Func<TSource, TKey3> keySelector3,
            Func<TSource, TKey4> keySelector4,
            Func<TSource, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>>();

            foreach (var item in source)
                dictionary.Add(keySelector1(item), keySelector2(item), keySelector3(item), keySelector4(item), valueSelector(item));

            return dictionary;
        }

        /// <summary>
        /// Add a value to a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="key4">The fourth key.</param>
        /// <param name="value">The value</param>
        public static void Add<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            TKey4 key4,
            TValue value)
        {
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            if (!cacheByKey1.TryGetValue(key1, out cacheByKey2))
                cacheByKey1.Add(key1, cacheByKey2 = new Dictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>());

            cacheByKey2.Add(key2, key3, key4, value);
        }

        /// <summary>
        /// Adds a dictionary to the last level of a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="value">The leaf dictionary.</param>
        public static void Add<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            IDictionary<TKey4, TValue> value)
        {
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            if (!cacheByKey1.TryGetValue(key1, out cacheByKey2))
                cacheByKey1.Add(key1, cacheByKey2 = new Dictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>());

            cacheByKey2.Add(key2, key3, value);
        }

        /// <summary>
        /// Try to get a value from a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="key4">The fourth key.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>If the key exists true, otherwise false.</returns>
        public static bool TryGetValue<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            TKey4 key4,
            out TValue value)
        {
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            if (cacheByKey1.TryGetValue(key1, out cacheByKey2))
                return cacheByKey2.TryGetValue(key2, key3, key4, out value);

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Try to get the leaf dictionary from a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="cacheByKey4">The leaf dictionary</param>
        /// <returns>If the key exists true, otherwise false.</returns>
        public static bool TryGetValue<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            out IDictionary<TKey4, TValue> cacheByKey4)
        {
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            if (cacheByKey1.TryGetValue(key1, out cacheByKey2))
                return cacheByKey2.TryGetValue(key2, key3, out cacheByKey4);

            cacheByKey4 = default(IDictionary<TKey4, TValue>);
            return false;
        }

        /// <summary>
        /// Discover if a nested dictionary of depth four contains a given key.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <returns>If the nested dictionary contains a given key then true, otherwise false.</returns>
        public static bool ContainsKey<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3)
        {
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            return cacheByKey1.TryGetValue(key1, out cacheByKey2) && cacheByKey2.ContainsKey(key2, key3);
        }

        /// <summary>
        /// Discover if a nested dictionary of depth four contains a given key.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="key4">The fourth key.</param>
        /// <returns>If the nested dictionary contains a given key then true, otherwise false.</returns>
        public static bool ContainsKey<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            TKey4 key4)
        {
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            IDictionary<TKey4, TValue> cacheByKey4;
            return cacheByKey1.TryGetValue(key1, out cacheByKey2) && cacheByKey2.TryGetValue(key2, key3, out cacheByKey4) && cacheByKey4.ContainsKey(key4);
        }

        /// <summary>
        /// Remove a value from a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="key4">The fourth key.</param>
        /// <returns>If the entry was deleted then true, otherwise false.</returns>
        public static bool Remove<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            TKey4 key4)
        {
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            if (cacheByKey1.TryGetValue(key1, out cacheByKey2))
            {
                if (cacheByKey2.Remove(key2, key3, key4))
                {
                    if (cacheByKey2.Count == 0)
                        cacheByKey1.Remove(key1);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get a value from a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="key4">The fourth key.</param>
        /// <returns>The value or throws an exception.</returns>
        public static TValue Get<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            TKey4 key4)
        {
            TValue value;
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            if (cacheByKey1.TryGetValue(key1, out cacheByKey2) && cacheByKey2.TryGetValue(key2, key3, key4, out value))
                return value;
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Sets a value in a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="cacheByKey1">The target dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="key4">The fourth key.</param>
        /// <param name="value">The value to set.</param>
        public static void Set<TKey1, TKey2, TKey3, TKey4, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            TKey4 key4,
            TValue value)
        {
            IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>> cacheByKey2;
            if (!cacheByKey1.TryGetValue(key1, out cacheByKey2))
                cacheByKey1.Add(key1, cacheByKey2 = new Dictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>());
            cacheByKey2.Set(key2, key3, key4, value);
        }

        /// <summary>
        /// The values in a nested dictionary of depth four.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TKey4">The type of the fourth key.</typeparam>
        /// <typeparam name="TValue">The type of the ultimate value.</typeparam>
        /// <param name="source">The target dictionary.</param>
        /// <returns>An enumeration of the values.</returns>
        public static IEnumerable<TValue> ToValues<TKey1, TKey2, TKey3, TKey4, TValue>(this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, IDictionary<TKey4, TValue>>>> source)
        {
            return source.SelectMany(a => a.Value.SelectMany(b => b.Value.SelectMany(c => c.Value.Select(d => d.Value))));
        }
    }
}
