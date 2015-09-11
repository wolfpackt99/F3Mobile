using F3.Business.Calendar;
using Google.Apis.Calendar.v3.Data;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace F3Mobile.Controllers
{
    [EnableCors(origins:"*",headers:"*", methods: "*")]
    public class CalendarController : ApiController
    {
        [Inject]
        public ICalendarBusiness CalendarBusiness { get; set; }
        // GET: api/Calendar
        public async Task<IEnumerable<CalendarListEntry>> Get()
        {
            var list = await CalendarBusiness.GetCalendarList();
            return list.Items;
        }

        // GET: api/Calendar/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Calendar
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Calendar/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Calendar/5
        public void Delete(int id)
        {
        }
    }
}
