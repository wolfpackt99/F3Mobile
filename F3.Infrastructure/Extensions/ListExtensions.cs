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
    }
}
