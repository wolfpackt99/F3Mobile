using System;
using System.Threading.Tasks;

namespace F3.Infrastructure.Cache
{
    public interface ICacheService
    {
        T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class;
        Task<T> GetOrSet<T>(string cacheKey, Func<Task<T>> getItemCallback) where T : class;
        void Remove(string cacheKey);
    }
}
