using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Shreeyan.Models
{
    public class CareerApplicationViewModel
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
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please select the area you're interested in.")]
        [Display(Name = "Area of interest")]
        public string RoleInterest { get; set; } = string.Empty;

        [Display(Name = "Years of experience")]
        public string? Experience { get; set; }

        [Display(Name = "LinkedIn / portfolio / resume link")]
        [StringLength(300)]
        public string? ProfileUrl { get; set; }

        [Display(Name = "Resume (PDF)")]
        public IFormFile? ResumeFile { get; set; }

        [Required(ErrorMessage = "Tell us a little about yourself.")]
        [StringLength(1000, ErrorMessage = "Please keep it under 1000 characters.")]
        [Display(Name = "Tell us about yourself")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Server-side check for the uploaded resume: PDF only, max 5 MB.
        /// Call this from the controller (IFormFile can't be validated with a simple
        /// DataAnnotation attribute without extra plumbing).
        /// </summary>
        public string? ValidateResumeFile()
        {
            if (ResumeFile == null)
            {
                return null; // upload is optional — ProfileUrl can be used instead
            }

            const long maxBytes = 5 * 1024 * 1024; // 5 MB
            if (ResumeFile.Length > maxBytes)
            {
                return "Resume file must be 5 MB or smaller.";
            }

            var extension = Path.GetExtension(ResumeFile.FileName);
            var isPdfExtension = string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase);
            var isPdfContentType = string.Equals(ResumeFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);

            if (!isPdfExtension || !isPdfContentType)
            {
                return "Resume must be a PDF file.";
            }

            return null;
        }
    }
}