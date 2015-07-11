using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3.ViewModels
{
    public class News
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public bool Show { get; set; }

        public bool Approved { get; set; }

        public int Order { get; set; }
    }
}
