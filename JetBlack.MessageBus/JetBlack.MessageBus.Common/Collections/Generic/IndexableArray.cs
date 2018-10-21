using System;
using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Common.Collections.Generic
{
    /// <summary>
    /// An n-dimensional array which can be indexed as if it was an n-dimensional array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IndexableArray<T>
    {
        private readonly ArrayIndexer _indexer;

        /// <summary>
        /// Constructs the indexed array given a 1-dimensional array and the n-dimensional bounds.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="bounds"></param>
        public IndexableArray(T[] array, params int[] bounds)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (bounds.Aggregate(1, (x, y) => x * y) != array.Length)
                throw new ArgumentOutOfRangeException(nameof(bounds), "There array length does not match the bounds");

            Array = array;
            _indexer = new ArrayIndexer(bounds);
        }

        /// <summary>
        /// The source array
        /// </summary>
        public T[] Array { get; }

        /// <summary>
        /// Index the array by n-dimensional indices.
        /// </summary>
        /// <param name="indices">The indices</param>
        /// <returns>The element located at the indices</returns>
        public T this[params int[] indices]
        {
            get { return Array[_indexer.ToIndex(indices)]; }
            set { Array[_indexer.ToIndex(indices)] = value; }
        }

        /// <summary>
        /// The bounds of the array.
        /// </summary>
        public IReadOnlyList<int> Bounds => _indexer.Bounds;
    }

    /// <summary>
    /// A factory for creating indexable arrays
    /// </summary>
    public static class IndexableArray
    {
        /// <summary>
        /// Creates an indexable array given a 1-dimensional array and the bounds
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="array">The 1-dimensional source array</param>
        /// <param name="bounds">The desired bounds of the n-dimensional array</param>
        /// <returns>An indexed array instance.</returns>
        public static IndexableArray<T> Create<T>(this T[] array, params int[] bounds)
        {
            return new IndexableArray<T>(array, bounds);
        }
    }
}
