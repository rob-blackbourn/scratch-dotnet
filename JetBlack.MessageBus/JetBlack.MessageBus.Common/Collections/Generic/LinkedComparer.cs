﻿using System;
using System.Collections.Generic;

namespace JetBlack.MessageBus.Common.Collections.Generic
{
    /// <summary>
    /// Comparer to daisy-chain two existing comparers and 
    /// apply in sequence (i.e. sort by x then y)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class LinkedComparer<T> : IComparer<T>
    {
        readonly IComparer<T> _primary, _secondary;
        /// <summary>
        /// Create a new LinkedComparer
        /// </summary>
        /// <param name="primary">The first comparison to use</param>
        /// <param name="secondary">The next level of comparison if the primary returns 0 (equivalent)</param>
        public LinkedComparer(IComparer<T> primary, IComparer<T> secondary)
        {
            if (primary == null)
                throw new ArgumentNullException(nameof(primary));
            if (secondary == null)
                throw new ArgumentNullException(nameof(secondary));

            _primary = primary;
            _secondary = secondary;
        }

        int IComparer<T>.Compare(T x, T y)
        {
            var result = _primary.Compare(x, y);
            return result == 0 ? _secondary.Compare(x, y) : result;
        }
    }
}
