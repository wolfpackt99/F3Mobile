using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailChimp.Lists;

namespace F3.Business.Fng
{
    [System.Runtime.Serialization.DataContract]
    public class NameVar : MergeVar
    {
        [System.Runtime.Serialization.DataMember(Name = "FNAME")]
        public string FirstName { get; set; }
        [System.Runtime.Serialization.DataMember(Name = "LNAME")]
        public string LastName { get; set; }
    }
}
