using Microsoft.AspNetCore.Http;
using Shreeyan.Models;

namespace Shreeyan.Services
{
    public interface IEmailSender
    {
        Task SendContactMessageAsync(ContactViewModel model);

        Task SendCareerApplicationAsync(CareerApplicationViewModel model, IFormFile? resumeFile);
    }
}