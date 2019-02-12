using System;
using System.Collections.Generic;

namespace GroupMeAnalysis.Util {
    //https://stackoverflow.com/questions/998066/linq-distinct-values
    public static class LinqExtensions {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }
    }
}