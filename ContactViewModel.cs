using System.ComponentModel.DataAnnotations;

namespace Shreeyan.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        [Display(Name = "Your name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Phone (optional)")]
        public string? Phone { get; set; }

        [Display(Name = "Company / business name (optional)")]
        [StringLength(100)]
        public string? Company { get; set; }

        [Required(ErrorMessage = "Please select a project type.")]
        [Display(Name = "Project type")]
        public string ProjectType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tell us a bit about your project.")]
        [StringLength(1000, ErrorMessage = "Please keep it under 1000 characters.")]
        [Display(Name = "Project details")]
        public string Message { get; set; } = string.Empty;
    }
}