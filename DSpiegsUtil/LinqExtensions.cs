using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil
{
    public static class LinqExtensions
    {
        //http://stackoverflow.com/questions/419019/split-list-into-sublists-with-linq
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static bool SetEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            var difference = first.SymmetricDifference(second, comparer);

            return !difference.Any();
        }

        public static IEnumerable<T> SymmetricDifference<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }

            if (second == null)
            {
                throw new ArgumentNullException("second");
            }

            var equalityComparer = comparer ?? EqualityComparer<T>.Default;

            IEnumerable<T> firstList = first as IList<T> ?? first.ToList();
            IEnumerable<T> secondList = second as IList<T> ?? second.ToList();

            var notInFirst = secondList.Except(firstList, equalityComparer);
            var notInSecond = firstList.Except(secondList, equalityComparer);

            var result = notInSecond.Union(notInFirst, equalityComparer);

            return result;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        public static void AddRange<T>(this BindingList<T> list, IEnumerable<T> addingList)
        {
            if (addingList == null)
            {
                return;
            }

            foreach (T addingItem in addingList)
            {
                list.Add(addingItem);
            }
        }

        //http://stackoverflow.com/questions/2085422/how-to-do-a-full-outer-join-in-linq
        public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
            where TInner : class
            where TOuter : class
        {
            var innerLookup = inner.ToLookup(innerKeySelector);
            var outerLookup = outer.ToLookup(outerKeySelector);

            var innerJoinItems = inner
                .Where(innerItem => !outerLookup.Contains(innerKeySelector(innerItem)))
                .Select(innerItem => resultSelector(null, innerItem));

            return outer
                .SelectMany(outerItem =>
                {
                    var innerItems = innerLookup[outerKeySelector(outerItem)];

                    return innerItems.Any() ? innerItems : new TInner[] { null };
                }, resultSelector)
                .Concat(innerJoinItems);
        }

        //http://stackoverflow.com/questions/7028919/concurrentdictionary-addorupdate
        // Either Add or overwrite        
        public static void AddOrUpdate<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key, TV value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }
    }
}
