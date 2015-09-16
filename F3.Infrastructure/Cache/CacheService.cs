using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace F3.Infrastructure.Cache
{
    public class CacheService: ICacheService
    {

        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            var item = MemoryCache.Default.Get(cacheKey) as T;
            if (item != null) return item;

            item = getItemCallback();
            MemoryCache.Default.Add(cacheKey, item, DateTime.UtcNow.AddMinutes(30));
            return item;
        }

        public async Task<T> GetOrSet<T>(string cacheKey, Func<Task<T>> getItemCallback) where T : class
        {
            var item = MemoryCache.Default.Get(cacheKey) as T;
            if (item != null) return item;

            item = await getItemCallback();
            MemoryCache.Default.Add(cacheKey, item, DateTime.UtcNow.AddMinutes(30));
            return item;
        }

        public void Remove(string cacheKey)
        {
            var removed = MemoryCache.Default.Remove(cacheKey);

        }
    }


}