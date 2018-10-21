using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBlack.MessageBus.Common.ComponentModel;
using JetBlack.MessageBus.Common.Linq;

namespace JetBlack.MessageBus.Common.Data
{
    /// <summary>
    /// Extension methods for data frames.
    /// </summary>
    public static class DataFrameExtensions
    {
        public static DataFrame PivotToDataFrame<TSource, TRow, TColumn, TValue>(
            this ICollection<TSource> source, 
            Expression<Func<TSource, TRow>> rowSelector, 
            Func<TSource, TColumn> columnSelector,
            Func<IEnumerable<TSource>, TValue> valueSelector)
        {
            var rowName = PropertySupport.ExtractPropertyName(rowSelector);
            var columns = source.Select(columnSelector).Distinct().ToList();
            var df = new DataFrame(new IVector[]{new Vector<TRow>(rowName)}.Concat(columns.Select(x => x.ToString()).Select(x => new Vector<TValue>(x))));

            var rows = source.GroupBy(rowSelector.Compile())
                .Select(rowGroup => new
                {
                    Key = rowGroup.Key,
                    Values = columns.GroupJoin(rowGroup, c => c, columnSelector, (c, values) => valueSelector(values))
                });

            foreach (var row in rows)
            {
                var items = new []{(object)row.Key}.Concat(row.Values.Cast<object>()).ToArray();
                df.Add(items);
            }

            return df;
        }

        public static DataFrame PivotToDataFrame<TSource, TRow, TColumn, TValue>(
            this ICollection<TSource> source,
            Func<TSource, TRow> rowSelector,
            string rowName,
            Func<TSource, TColumn> columnSelector,
            Func<IEnumerable<TSource>, TValue> valueSelector)
        {
            var columns = source.Select(columnSelector).Distinct().ToList();
            var df = new DataFrame(new IVector[] { new Vector<TRow>(rowName) }.Concat(columns.Select(x => x.ToString()).Select(x => new Vector<TValue>(x))));

            var rows = source.GroupBy(rowSelector)
                .Select(rowGroup => new
                {
                    Key = rowGroup.Key,
                    Values = columns.GroupJoin(rowGroup, c => c, columnSelector, (c, values) => valueSelector(values))
                });

            foreach (var row in rows)
            {
                var items = new[] { (object)row.Key }.Concat(row.Values.Cast<object>()).ToArray();
                df.Add(items);
            }

            return df;
        }

        /// <summary>
        /// Pivot a data frame.
        /// </summary>
        /// <param name="dataFrame">The data frame to pivot.</param>
        /// <param name="rowName">The row column</param>
        /// <param name="columnName">The column</param>
        /// <param name="valueName">The value column.</param>
        /// <returns></returns>
        public static DataFrame PivotToDataFrame(this DataFrame dataFrame, string rowName, string columnName, string valueName)
        {
            var rows = dataFrame[rowName].Cast<object>();
            var columns = dataFrame[columnName].Cast<object>();
            var values = dataFrame[valueName].Cast<object>();
            var source = rows.Zip(columns, values, (r, c, v) => new {r, c, v}).ToList();
            return source.PivotToDataFrame(x => x.r, rowName, x => x.c, x => x.First().v);
        }
    }
}
