using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using F3.Business.Calendar;
using F3Mobile.Code;
using Google.Apis.Calendar.v3.Data;
using Ninject;

namespace F3Mobile.Controllers
{
    public partial class CalendarController : Controller
    {
        [Inject]
        public ICalendarBusiness CalendarBusiness { get; set; }

        [Inject]
        public ICacheService Cache { get; set; }
        
        [HttpGet]
        public virtual async Task<ActionResult> Get(string id, bool all = true, bool bust = false)
        {
            var cacheKey = string.Format("{0}-{1}-{2}", "calenderitems", id, all.ToString());
            if (bust)
            {
                Cache.Remove(cacheKey);
            }
            var events =
                await
                    Cache.GetOrSet(cacheKey,
                        async () => await CalendarBusiness.GetEvents(id, all));


            return Json(events, JsonRequestBehavior.AllowGet);
        }

        public virtual async Task<ActionResult> List(bool bust = false)
        {
            if (bust)
            {
                Cache.Remove("CalList");
            }
            var events = await Cache.GetOrSet("CalList", async () => await CalendarBusiness.GetCalendarList());
            return Json(events, JsonRequestBehavior.AllowGet);
        }

    }
}
