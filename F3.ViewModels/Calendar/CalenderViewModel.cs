using System;
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

        public string Type { get; set; }

        public IEnumerable<EventViewModel> Items { get; set; } 

        public MetaDataViewModel MetaData { get; set; }
    }
}
