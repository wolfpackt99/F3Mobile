using F3.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System;

namespace F3.ViewModels
{
    public class Contact
    {
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStrings), Name = "FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(ResourceType = typeof(ResourceStrings), Name = "LastName")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(ResourceType = typeof(ResourceStrings), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(ResourceType = typeof(ResourceStrings), Name = "F3Name")]
        public string F3Name { get; set; } = string.Empty;

        [Display(ResourceType = typeof(ResourceStrings), Name = "Workout")]
        public string Workout { get; set; } = string.Empty;

        [Display(ResourceType = typeof(ResourceStrings), Name = "EH")]
        public string EH { get; set; } = string.Empty;

        [Display(ResourceType = typeof(ResourceStrings), Name = "Twitter")]
        public string Twitter { get; set; } = string.Empty;
        public DateTime SignupDate { get; set; } = DateTime.UtcNow;

        
    }
}
