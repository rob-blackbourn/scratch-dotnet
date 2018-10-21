using System;
using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Common.Collections
{
    /// <summary>
    /// An n-dimensional array indexer
    /// </summary>
    public class ArrayIndexer
    {
        private readonly int[] _sum;

        /// <summary>
        /// Constructs an n-dimensional array indexer given tyhe bounds.
        /// </summary>
        /// <param name="bounds">The bounds of the array</param>
        public ArrayIndexer(params int[] bounds)
        {
            _sum = ComputeBoundsSums(bounds); // Pre-compute bounds sums for speed.
            Bounds = Array.AsReadOnly(bounds);
        }

        /// <summary>
        /// The bounds of the indexer.
        /// </summary>
        public IReadOnlyList<int> Bounds { get; }

        /// <summary>
        /// Converts a given n-dimensional index to a 1-dimensional index.
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public int ToIndex(params int[] indices)
        {
            if (indices.Length != Bounds.Count)
                throw new ArgumentException("There should be as many indices as bounds", nameof(indices));
            if (indices.Where((x, i) => x < 0 || x >= Bounds[i]).Any())
                throw new IndexOutOfRangeException();

            var index = indices[0];
            for (int i = 1; i < indices.Length; ++i)
                index += _sum[i - 1] * indices[i];
            return index;
        }

        /// <summary>
        /// Converts a 1-dimensional index into an n-dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int[] FromIndex(int index)
        {
            var indices = new int[Bounds.Count];
            for (var i = Bounds.Count - 1; i > 0; --i)
            {
                indices[i] = index / _sum[i - 1];
                index %= _sum[i - 1];
            }
            indices[0] = index;

            return indices;
        }

        private static int[] ComputeBoundsSums(int[] bounds)
        {
            var array = new int[bounds.Length - 1];
            for (int i = 1, sum = bounds[i - 1]; i < bounds.Length; ++i, sum *= bounds[i - 1])
                array[i - 1] = sum;
            return array;
        }
    }

    /// <summary>
    /// Extension methods for the array indexer.
    /// </summary>
    public static class ArrayIndexerExtensions
    {
        /// <summary>
        /// Returns the element for the given index.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array</typeparam>
        /// <param name="arrayIndexer">The array indexer</param>
        /// <param name="array">The array</param>
        /// <param name="indices">The array indices.</param>
        /// <returns>The element at the location specified by the supplied indices.</returns>
        public static T Index<T>(this ArrayIndexer arrayIndexer, T[] array, params int[] indices)
        {
            if (arrayIndexer == null)
                throw new ArgumentNullException(nameof(arrayIndexer));
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            return array[arrayIndexer.ToIndex(indices)];
        }

        /// <summary>
        /// Returns the element for the given index.
        /// </summary>
        /// <param name="arrayIndexer">The array indexer</param>
        /// <param name="array">The array</param>
        /// <param name="indices">The array indices.</param>
        /// <returns>The element at the location specified by the supplied indices.</returns>
        public static object Index(this ArrayIndexer arrayIndexer, Array array, params int[] indices)
        {
            if (arrayIndexer == null)
                throw new ArgumentNullException(nameof(arrayIndexer));
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            return array.GetValue(arrayIndexer.ToIndex(indices));
        }
    }
}
