using System;
using System.Collections;
using System.Collections.Generic;

namespace JetBlack.MessageBus.Common.Collections.Generic
{
    /// <summary>
    /// A factory class for creating ranges.
    /// </summary>
    public static class Range
    {
        /// <summary>
        /// Create a new range.
        /// </summary>
        /// <typeparam name="T">The type of the range</typeparam>
        /// <param name="start">The start of the range</param>
        /// <param name="end">The end of the range</param>
        /// <param name="comparer">A comparer for elements in the range</param>
        /// <param name="isStartIncluded">If the start is included in the range true, otherwise false.</param>
        /// <param name="isEndIncluded">If the end is included in the range true, otherwise false.</param>
        /// <returns></returns>
        public static Range<T> Create<T>(T start, T end, IComparer<T> comparer = null, bool isStartIncluded = true, bool isEndIncluded = true)
        {
            return new Range<T>(start, end, comparer, isStartIncluded, isEndIncluded);
        }
    }

    /// <summary>
    /// A class which represents and range.
    /// </summary>
    /// <typeparam name="T">The type of the range.</typeparam>
    public struct Range<T>
    {
        /// <summary>
        /// Constructs a new range, including or excluding each end as specified,
        /// with the given comparer.
        /// </summary>
        public Range(T start, T end, IComparer<T> comparer = null, bool isStartIncluded = true, bool isEndIncluded = true)
        {
            Comparer = comparer ?? Comparer<T>.Default;
            IsAscending = Comparer.Compare(start, end) <= 0;
            Start = start;
            End = end;
            IsStartIncluded = isStartIncluded;
            IsEndIncluded = isEndIncluded;
        }

        /// <summary>
        /// If the range is ascending true, otherwise false.
        /// </summary>
        public bool IsAscending { get; }

        /// <summary>
        /// The start of the range.
        /// </summary>
        public T Start { get; }

        /// <summary>
        /// The end of the range.
        /// </summary>
        public T End { get; }

        /// <summary>
        /// The comparer
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// Whether or not this range includes the start point
        /// </summary>
        public bool IsStartIncluded { get; }

        /// <summary>
        /// Whether or not this range includes the end point
        /// </summary>
        public bool IsEndIncluded { get; }

        /// <summary>
        /// Returns a range with the same boundaries as this, but excluding the end point.
        /// When called on a range already excluding the end point, the original range is returned.
        /// </summary>
        public Range<T> ExcludeEnd()
        {
            return !IsEndIncluded ? this : new Range<T>(Start, End, Comparer, IsStartIncluded, false);
        }

        /// <summary>
        /// Returns a range with the same boundaries as this, but excluding the start point.
        /// When called on a range already excluding the start point, the original range is returned.
        /// </summary>
        public Range<T> ExcludeStart()
        {
            return !IsStartIncluded ? this : new Range<T>(Start, End, Comparer, false, IsEndIncluded);
        }

        /// <summary>
        /// Returns a range with the same boundaries as this, but including the end point.
        /// When called on a range already including the end point, the original range is returned.
        /// </summary>
        public Range<T> IncludeEnd()
        {
            return IsEndIncluded ? this : new Range<T>(Start, End, Comparer, IsStartIncluded, true);
        }

        /// <summary>
        /// Returns a range with the same boundaries as this, but including the start point.
        /// When called on a range already including the start point, the original range is returned.
        /// </summary>
        public Range<T> IncludeStart()
        {
            return IsStartIncluded ? this : new Range<T>(Start, End, Comparer, true, IsEndIncluded);
        }

        /// <summary>
        /// Returns whether or not the range contains the given value
        /// </summary>
        public bool Contains(T value)
        {
            var lowerBound = Comparer.Compare(value, Start);
            if (lowerBound < 0 || (lowerBound == 0 && !IsStartIncluded))
                return false;
            var upperBound = Comparer.Compare(value, End);
            return upperBound < 0 || (upperBound == 0 && IsEndIncluded);
        }

        /// <summary>
        /// returns whether the range contains another range.
        /// </summary>
        /// <param name="range">The other range</param>
        /// <returns>If the range contains the other range then true; otherwise false.</returns>
        public bool Contains(Range<T> range)
        {
            return Contains(range.Start) && Contains(range.End);
        }

        /// <summary>
        /// returns whether the range intersects another range.
        /// </summary>
        /// <param name="range">The other range</param>
        /// <returns>If the range intersects the other range then true; otherwise false.</returns>
        public bool Intersects(Range<T> range)
        {
            return Contains(range.Start) || Contains(range.End) || range.Contains(Start) || range.Contains(End);
        }

        /// <summary>
        /// Returns an iterator which begins at the start of this range,
        /// applying the given step delegate on each iteration until the 
        /// end is reached or passed. The start and end points are included
        /// or excluded according to this range.
        /// </summary>
        /// <param name="step">Delegate to apply to the "current value" on each iteration</param>
        public IEnumerable<T> AsEnumerable(Func<T, T> step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            if ((IsAscending && Comparer.Compare(Start, step(Start)) >= 0) || (!IsAscending && Comparer.Compare(End, step(End)) <= 0))
                throw new ArgumentException("step does nothing, or progresses the wrong way");

            return new RangeIterator(this, step);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Start} : {End}";
        }

        private class RangeIterator : IEnumerable<T>
        {
            private readonly Range<T> _range;
            private readonly Func<T, T> _step;

            public RangeIterator(Range<T> range, Func<T, T> step)
            {
                _range = range;
                _step = step;
            }

            public IEnumerator<T> GetEnumerator()
            {
                // A descending range effectively has the start and end points (and inclusions)
                // reversed, and a reverse comparer.
                var isStartIncluded = _range.IsAscending ? _range.IsStartIncluded : _range.IsEndIncluded;
                var isEndIncluded = _range.IsAscending ? _range.IsEndIncluded : _range.IsStartIncluded;
                var comparer = _range.IsAscending ? _range.Comparer : _range.Comparer.Reverse();

                // Now we can use our local version of the range variables to iterate

                var value = _range.Start;

                // Deal with possibility that start point = end point
                if (isStartIncluded)
                    if (isEndIncluded || comparer.Compare(value, _range.End) < 0)
                        yield return value;

                value = _step(value);

                while (comparer.Compare(value, _range.End) < 0)
                {
                    yield return value;
                    value = _step(value);
                }

                // We've already performed a step, therefore we can't
                // still be at the start point
                if (isEndIncluded && comparer.Compare(value, _range.End) == 0)
                    yield return value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }

}
