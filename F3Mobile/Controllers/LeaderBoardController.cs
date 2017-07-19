using System.Threading.Tasks;
using System.Web.Mvc;
using F3.Business.Leaderboard;
using F3Mobile.Code;
using Ninject;

namespace F3Mobile.Controllers
{
    public partial class LeaderBoardController : Controller
    {
        [Inject]
        public IStravaBusiness StravaBusiness { get; set; }

        [Inject]
        public ICacheService Cache { get; set; }

        // GET: LeaderBoard
        [HttpGet]
        public virtual async Task<ActionResult> _Index(bool clear = false)
        {
            var x = new StravaBusiness();
            if (clear == false)
            {
                Cache.Remove("stats");
            }

            var stats = await Cache.GetOrSet("stats", async () => await x.GetData());

            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public async Task<JsonResult> SetAuth(string code)
        {
            var token = await StravaBusiness.GetAuthToken(code);
            return Json(token, JsonRequestBehavior.AllowGet);
        }
    }
}