using System;
using F3.Business;
using F3.Business.News;
using F3.ViewModels;
using F3Mobile.Code;
using Ninject;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject.Activation.Caching;
using System.Net;

namespace F3Mobile.Controllers
{
    public partial class HomeController : AsyncController
    {
        private const string Alert = "Alert";
        private const string LatestAdds = "LatestAdds";
        [Inject]
        public ISubscribe ContactBiz { get; set; }

        [Inject]
        public ICacheService Cache { get; set; }

        [Inject]
        public INewsBusiness News { get; set; }

        public virtual async Task<ActionResult> Index()
        {
            return RedirectPermanent("http://f3southcharlotte.com");
            //var news = await News.GetNews();
            ////var news = await Cache.GetOrSet("News", async () => await News.GetNews());
            //return View(news);
        }
    }
}