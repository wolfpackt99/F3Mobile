using System;
using F3.Business;
using F3.ViewModels;
using Ninject;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace F3Mobile.Controllers
{
    public partial class HomeController : Controller
    {
        private const string Alert = "Alert";
        [Inject]
        public ISubscribe ContactBiz { get; set; }

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Fng()
        {
            ViewBag.Message = "FNG Signup";
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
    }
}