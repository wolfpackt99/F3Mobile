using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3.ViewModels.Calendar
{
    public class QRequest
    {
        [Required]
        [EmailAddress]
        public string From { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Site { get; set; }
    }
}
