using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F3.ViewModels;

namespace F3.Business.News
{
    public interface INewsBusiness
    {
        Task<IEnumerable<F3.ViewModels.News>> GetNews();
        Task<bool> AddNews(F3.ViewModels.News news);
    }
}
