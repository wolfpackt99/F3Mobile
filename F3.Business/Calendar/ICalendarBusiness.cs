using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;

namespace F3.Business.Calendar
{
    public interface ICalendarBusiness
    {
        Task<Events> GetEvents(string id, bool all = true);
        Task<CalendarList> GetCalendarList();
        Task<IEnumerable<Events>> GetAllEvents(bool all = true, bool bust = false);
    }
}
