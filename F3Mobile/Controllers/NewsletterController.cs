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
    public partial class NewsletterController : AsyncController
    {
        private const string Alert = "Alert";
        private const string LatestAdds = "LatestAdds";
        [Inject]
        public ISubscribe ContactBiz { get; set; }

        [Inject]
        public ICacheService Cache { get; set; }

        [Inject]
        public INewsBusiness News { get; set; }

        

        public virtual ActionResult Index()
        {
            ViewBag.Message = "FNG/Newsletter";
            ViewBag.Alert = TempData[Alert];
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<ActionResult> Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var subscribed = await ContactBiz.Add(contact);
                    TempData[Alert] = "FNG Added";
                    Cache.Remove(LatestAdds);
                    return RedirectToAction(MVC.Newsletter.Actions.Index());
                }
                catch (Exception exp)
                {
                    ModelState.AddModelError("",exp.Message);
                }
            }
            return View(contact);
        }

        [HttpGet]
        public async Task<JsonResult> GetRecent()
        {
            var latest = await Cache.GetOrSet(LatestAdds, async () => await ContactBiz.Latest());
            return Json(latest, JsonRequestBehavior.AllowGet);
        }
    }
}