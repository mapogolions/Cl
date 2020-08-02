using System;
using System.Collections.Generic;

namespace Cl.Extensions
{
    public static class IEnumerableOps
    {
        public static void ForEach<A>(this IEnumerable<A> @this, Action<A> apply)
        {
            foreach (var item in @this) apply(item);
        }

        public static IEnumerable<(A First, B Second)> ZipIfBalanced<A, B>(this IEnumerable<A> @this, IEnumerable<B> that)
        {
            var first = @this.GetEnumerator();
            var second = that.GetEnumerator();
            while (true)
            {
                var firstIsExhausted = !first.MoveNext();
                var secondIsExhausted = !second.MoveNext();
                if (firstIsExhausted && secondIsExhausted) yield break;
                if (!firstIsExhausted && !secondIsExhausted)
                {
                    yield return (first.Current, second.Current);
                    continue;
                }
                throw new InvalidOperationException("Unbalanced");
            }
        }

        public static T FirstOrLastDefault<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
            var current = default(T);
            foreach (var item in @this)
            {
                current = item;
                if (predicate(current)) break;
            }
            return current;
        }
    }
}
