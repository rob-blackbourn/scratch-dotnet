using System;
using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Common.Collections.Generic
{
    /// <summary>
    /// Extension methods for three key dictionaries.
    /// </summary>
    public static class ThreeKeyDictionaryExtensions
    {
        /// <summary>
        /// Create a three key dictionary.
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the source collection.</typeparam>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source collection from which the dictionary is built.</param>
        /// <param name="keySelector1">The selector for the first key.</param>
        /// <param name="keySelector2">The selector for the second key.</param>
        /// <param name="keySelector3">The selector for the third key.</param>
        /// <param name="valueSelector">THe selector for the value.</param>
        /// <returns>A three key dictionary.</returns>
        public static Dictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> ToDictionary<TSource, TKey1, TKey2, TKey3, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey1> keySelector1,
            Func<TSource, TKey2> keySelector2,
            Func<TSource, TKey3> keySelector3,
            Func<TSource, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>>();

            foreach (var item in source)
                dictionary.Add(keySelector1(item), keySelector2(item), keySelector3(item), valueSelector(item));

            return dictionary;
        }

        /// <summary>
        /// Adds a value to a three key dictionary.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <typeparam name="TKey3">The type of the third key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictByKey1">A three key dictionary.</param>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="key3">The third key.</param>
        /// <param name="value">The value to add.</param>
        public static void Add<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> dictByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            TValue value)
        {
            IDictionary<TKey2, IDictionary<TKey3, TValue>> dictByKey2;
            if (!dictByKey1.TryGetValue(key1, out dictByKey2))
                dictByKey1.Add(key1, dictByKey2 = new Dictionary<TKey2, IDictionary<TKey3, TValue>>());

            dictByKey2.Add(key2, key3, value);
        }

        public static void Add<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> dictByKey1,
            TKey1 key1,
            TKey2 key2,
            IDictionary<TKey3, TValue> value)
        {
            IDictionary<TKey2, IDictionary<TKey3, TValue>> dictByKey2;
            if (!dictByKey1.TryGetValue(key1, out dictByKey2))
                dictByKey1.Add(key1, dictByKey2 = new Dictionary<TKey2, IDictionary<TKey3, TValue>>());

            dictByKey2.Add(key2, value);
        }

        public static bool TryGetValue<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            out TValue value)
        {
            IDictionary<TKey2, IDictionary<TKey3, TValue>> cacheByKey2;
            if (cacheByKey1.TryGetValue(key1, out cacheByKey2))
                return cacheByKey2.TryGetValue(key2, key3, out value);

            value = default(TValue);
            return false;
        }

        public static bool TryGetValue<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            out IDictionary<TKey3, TValue> cacheByKey3)
        {
            IDictionary<TKey2, IDictionary<TKey3, TValue>> cacheByKey2;
            if (cacheByKey1.TryGetValue(key1, out cacheByKey2))
                return cacheByKey2.TryGetValue(key2, out cacheByKey3);

            cacheByKey3 = default(IDictionary<TKey3, TValue>);
            return false;
        }

        public static bool ContainsKey<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2)
        {
            IDictionary<TKey2, IDictionary<TKey3, TValue>> cacheByKey2;
            return cacheByKey1.TryGetValue(key1, out cacheByKey2) && cacheByKey2.ContainsKey(key2);
        }

        public static bool ContainsKey<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3)
        {
            IDictionary<TKey2, IDictionary<TKey3, TValue>> cacheByKey2;
            IDictionary<TKey3, TValue> cacheByKey3;
            return cacheByKey1.TryGetValue(key1, out cacheByKey2) && cacheByKey2.TryGetValue(key2, out cacheByKey3) && cacheByKey3.ContainsKey(key3);
        }

        public static bool Remove<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3)
        {
            IDictionary<TKey2, IDictionary<TKey3, TValue>> cacheByKey2;
            if (cacheByKey1.TryGetValue(key1, out cacheByKey2))
            {
                if (cacheByKey2.Remove(key2, key3))
                {
                    if (cacheByKey2.Count == 0)
                        cacheByKey1.Remove(key1);
                    return true;
                }
            }

            return false;
        }

        public static TValue Get<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3)
        {
            TValue value;
            IDictionary<TKey2, IDictionary<TKey3, TValue>> cacheByKey2;
            if (cacheByKey1.TryGetValue(key1, out cacheByKey2) && cacheByKey2.TryGetValue(key2, key3, out value))
                return value;
            throw new KeyNotFoundException();
        }

        public static void Set<TKey1, TKey2, TKey3, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> cacheByKey1,
            TKey1 key1,
            TKey2 key2,
            TKey3 key3,
            TValue value)
        {
            IDictionary<TKey2, IDictionary<TKey3, TValue>> cacheByKey2;
            if (!cacheByKey1.TryGetValue(key1, out cacheByKey2))
                cacheByKey1.Add(key1, cacheByKey2 = new Dictionary<TKey2, IDictionary<TKey3, TValue>>());
            cacheByKey2.Set(key2, key3, value);
        }

        public static IEnumerable<TValue> ToValues<TKey1, TKey2, TKey3, TValue>(this IDictionary<TKey1, IDictionary<TKey2, IDictionary<TKey3, TValue>>> source)
        {
            return source.SelectMany(a => a.Value.SelectMany(b => b.Value.Select(c => c.Value)));
        }
    }
}
