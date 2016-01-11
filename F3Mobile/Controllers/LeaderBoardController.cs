using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using F3.Business.Leaderboard;
using F3Mobile.Code;
using Ninject;

namespace F3Mobile.Controllers
{
    public class LeaderBoardController : Controller
    {
        [Inject]
        public ICacheService Cache { get; set; }

        // GET: LeaderBoard
        [HttpGet]
        public async Task<ActionResult> _Index()
        {
            var x = new StravaBusiness();

            var stats = await Cache.GetOrSet("stats", async () => await x.GetData());

            return Json(stats, JsonRequestBehavior.AllowGet);
        }
    }
}