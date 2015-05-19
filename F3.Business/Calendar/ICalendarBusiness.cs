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
        Task<Events> GetEvents(string id);
        Task<CalendarList> GetCalendarList();
    }
}
