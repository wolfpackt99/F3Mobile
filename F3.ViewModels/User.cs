using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3.ViewModels
{
    public class User
    {
        public long StravaId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public int ActivityCount { get; set; }
        public decimal TotalMiles { get; set; }
        
        public string ProfilePic { get; set; }
        public List<Stat> Stats { get; set; }
        public decimal Running { get; set; }
    }
}
