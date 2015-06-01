using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using F3.Business.Calendar;
using Google.Apis.Calendar.v3.Data;
using Ninject;

namespace F3Mobile.Controllers
{
    public partial class CalendarController : Controller
    {
        [Inject]
        public ICalendarBusiness CalendarBusiness { get; set; }
        
        [HttpGet]
        public virtual async Task<ActionResult> Get(string id, bool all = true)
        {
            return Json(await CalendarBusiness.GetEvents(id, all), JsonRequestBehavior.AllowGet);
        }

    }
}
