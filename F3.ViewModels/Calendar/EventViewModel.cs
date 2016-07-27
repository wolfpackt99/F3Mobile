using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3.ViewModels.Calendar
{
    public class EventViewModel
    {
        public string CalendarId { get; set; }
        public string Title { get; set; }
        public DateTime? Start { get; set; }

        public string Description { get; set; }
        public string Date { get; set; }
        public string CalendarName { get; set; }
        public string Preblast { get; set; }
        public string Tag { get; set; }
        public string Location { get; set; }

        public string Time { get; set; }
        public string Region { get; set; }
        public string Type { get; set; }
    }
}
