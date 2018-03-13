﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3.ViewModels.Calendar
{
    public class CalenderViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Group { get; set; }
        public string Url { get; set; }
        public string Q { get; set; }
        public string Region { get; set; }
        public string Notes { get; set; }
        public int DayOfWeek { get; set; }
        public bool Show { get; set; }
        public string CalendarID { get; set; }
        public string DisplayLocation { get; set; }
        public string LocationHint { get; set; }

        public string Type { get; set; }

        public IEnumerable<EventViewModel> Items { get; set; } 

        public MetaDataViewModel MetaData { get; set; }
        public string TimeZone { get; set; }
    }
}
