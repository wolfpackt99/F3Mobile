using F3.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace F3.ViewModels
{
    public class Contact
    {
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStrings), Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStrings), Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStrings), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStrings), Name = "F3Name")]
        public string F3Name { get; set; }

        [Display(ResourceType = typeof(ResourceStrings), Name = "Workout")]
        public string Workout { get; set; }

    }
}
