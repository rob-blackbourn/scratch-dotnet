using System;

namespace JetBlack.MessageBus.Common.Collections
{
    /// <summary>
    /// Extension methods for working with arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Creates an n-dimensional array from a 1-dimension array given the bounds.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="array1D">A 1-dimensional source array</param>
        /// <param name="bounds">The bounds of the desired array</param>
        /// <returns>An n-dimensional array.</returns>
        public static Array CreateArray<T>(this T[] array1D, params int[] bounds)
        {
            if (array1D == null)
                throw new ArgumentNullException(nameof(array1D));

            var arrayNd = Array.CreateInstance(typeof(T), bounds);

            var indices = new int[bounds.Length];
            foreach (var value in array1D)
            {
                arrayNd.SetValue(value, indices);

                for (var j = 0; j < bounds.Length; ++j)
                {
                    if (++indices[j] < bounds[j])
                        break;
                    indices[j] = 0;
                }
            }

            return arrayNd;
        }

        /// <summary>
        /// Indexes a 1-dimensional array as an n-dimensional array given the bounds and indices.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="bounds"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public static T Index<T>(this T[] array, int[] bounds, params int[] indices)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (bounds == null)
                throw new ArgumentNullException(nameof(bounds));
            if (indices.Length == 0 || indices.Length != bounds.Length)
                throw new ArgumentException("There should be at least one index and as many indices as bounds", nameof(indices));

            var index = indices[0];
            for (int i = 1, sum = bounds[i - 1]; i < indices.Length; ++i, sum *= bounds[i - 1])
                index += sum * indices[i];
            return array[index];
        }

        /// <summary>
        /// Indexes an array given the bounds and indices.
        /// </summary>
        /// <param name="array">The source array</param>
        /// <param name="bounds">The bounds</param>
        /// <param name="indices">The indices</param>
        /// <returns>The object at the given index location.</returns>
        public static object Index(this Array array, int[] bounds, params int[] indices)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException("The array must be one dimensional", nameof(array));
            if (bounds == null)
                throw new ArgumentNullException(nameof(bounds));
            if (indices.Length == 0 || indices.Length != bounds.Length)
                throw new ArgumentException("There should be at least one index and as many indices as bounds", nameof(indices));

            var index = indices[0];
            for (int i = 1, sum = bounds[i - 1]; i < indices.Length; ++i, sum *= bounds[i - 1])
                index += sum * indices[i];
            return array.GetValue(index);
        }
    }
}
