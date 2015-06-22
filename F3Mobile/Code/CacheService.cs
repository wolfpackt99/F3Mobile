using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;
using F3Mobile.App_Start;
using System.Runtime.Caching;

namespace F3Mobile.Code
{
    public class CacheService: ICacheService
    {

        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            var item = MemoryCache.Default.Get(cacheKey) as T;
            if (item != null) return item;

            item = getItemCallback();
            MemoryCache.Default.Add(cacheKey, item, DateTime.UtcNow.AddMinutes(10));
            return item;
        }

        public async Task<T> GetOrSet<T>(string cacheKey, Func<Task<T>> getItemCallback) where T : class
        {
            var item = MemoryCache.Default.Get(cacheKey) as T;
            if (item != null) return item;

            item = await getItemCallback();
            MemoryCache.Default.Add(cacheKey, item, DateTime.UtcNow.AddMinutes(10));
            return item;
        }

        public void Remove(string cacheKey)
        {
            var removed = MemoryCache.Default.Remove(cacheKey);

        }
    }


}