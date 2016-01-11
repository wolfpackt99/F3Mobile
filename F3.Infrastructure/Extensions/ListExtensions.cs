using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebSockets;

namespace F3.Infrastructure.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<Task<R>> SelectAsync<T, R>(this IEnumerable<T> source, Func<T, Task<R>> func)
        {
            return source.Select(func);
        }

        public static Task<R[]> ForkJoin<T, R>(this IEnumerable<T> source, Func<T, Task<R>> worker)
        {
            var futures = source.Select(worker).ToArray();

            return Task.WhenAll<R>(futures);
        }
    }
}
