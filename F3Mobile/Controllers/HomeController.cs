using System;
using F3.Business;
using F3.Business.News;
using F3.ViewModels;
using F3Mobile.Code;
using Ninject;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject.Activation.Caching;

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
            var news = await Cache.GetOrSet("News", async () => await News.GetNews());
            return View(news);
        }

        public virtual ActionResult Fng()
        {
            ViewBag.Message = "FNG/Newsletter Signup";
            ViewBag.Alert = TempData[Alert];
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<ActionResult> Fng(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var subscribed = await ContactBiz.Add(contact);
                    TempData[Alert] = "FNG Added";
                    Cache.Remove(LatestAdds);
                    return RedirectToAction(MVC.Home.Actions.Fng());
                }
                catch (Exception exp)
                {
                    ModelState.AddModelError("",exp.Message);
                }
            }
            return View(contact);
        }

        public virtual ActionResult Schedule()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetRecent()
        {
            var latest = await Cache.GetOrSet(LatestAdds, async () => await ContactBiz.Latest());
            return Json(latest, JsonRequestBehavior.AllowGet);
        }
    }
}