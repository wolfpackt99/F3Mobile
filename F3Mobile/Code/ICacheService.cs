using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3Mobile.Code
{
    public interface ICacheService
    {
        T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class;
        Task<T> GetOrSet<T>(string cacheKey, Func<Task<T>> getItemCallback) where T : class;
        void Remove(string cacheKey);
    }
}
