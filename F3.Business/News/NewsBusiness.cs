using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseSharp.Portable;
using Newtonsoft.Json;

namespace F3.Business.News
{
    /// <summary>
    /// 
    /// </summary>
    public class NewsBusiness : INewsBusiness
    {
        /// <summary>
        /// Gets the news.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ViewModels.News>> GetNews()
        {
            var rootUri = ConfigurationManager.AppSettings.Get("FirebaseUri");
            var authToken = ConfigurationManager.AppSettings.Get("FirebaseAuthToken");
            var fb = new Firebase(rootUri, authToken);

            var data = await fb.GetAsync("news");
            
            var result = JsonConvert.DeserializeObject<IEnumerable<ViewModels.News>>(data);
            var select = result.Where(CurrentNews).OrderBy(s => s.Order);

            
            return select;
        }

        private bool CurrentNews(ViewModels.News arg)
        {
            if (arg == null) return false;

            var now = DateTime.Now;
            if ((now >= arg.Start && now <= arg.End) && arg.Show == true)
            {
                return true;
            }
            return false;
        }

    }
}
