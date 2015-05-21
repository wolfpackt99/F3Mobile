using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using F3.Business;
using Ninject;

namespace F3Mobile.Controllers
{
    public partial class BackblastController : Controller
    {
        [Inject]
        public IFeed Feed { get; set; }
        // GET: Backblast
        public virtual ActionResult Index()
        {
            var posts = Feed.GetPosts();
            return View(posts);
        }
    }
}