using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shreeyan.Models;

namespace Shreeyan.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendContactMessageAsync(ContactViewModel model)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = $"New enquiry — {model.ProjectType} ({model.Name})",
                Body = BuildContactBody(model),
                IsBodyHtml = false
            };

            message.To.Add(_settings.OwnerEmail);

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                message.ReplyToList.Add(new MailAddress(model.Email, model.Name));
            }

            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword)
            };

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Contact email sent for {Email}", model.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact email for {Email}", model.Email);
                throw;
            }
        }

        public async Task SendCareerApplicationAsync(CareerApplicationViewModel model, IFormFile? resumeFile)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = $"New career application — {model.RoleInterest} ({model.Name})",
                Body = BuildCareerBody(model, resumeFile),
                IsBodyHtml = false
            };

            message.To.Add(_settings.OwnerEmail);

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                message.ReplyToList.Add(new MailAddress(model.Email, model.Name));
            }

            // Attach the uploaded resume, if any. The stream is opened here and
            // disposed via the Attachment/MailMessage lifecycle below.
            Stream? resumeStream = null;
            if (resumeFile is { Length: > 0 })
            {
                resumeStream = resumeFile.OpenReadStream();
                var attachment = new Attachment(resumeStream, resumeFile.FileName, resumeFile.ContentType);
                message.Attachments.Add(attachment);
            }

            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword)
            };

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Career application email sent for {Email}", model.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send career application email for {Email}", model.Email);
                throw;
            }
            finally
            {
                resumeStream?.Dispose();
            }
        }

        private static string BuildContactBody(ContactViewModel model)
        {
            return
                $"""
                 New contact form submission from the Shreeyan Webtech website.

                 Name: {model.Name}
                 Email: {model.Email}
                 Phone: {model.Phone ?? "-"}
                 Company: {model.Company ?? "-"}
                 Project type: {model.ProjectType}

                 Message:
                 {model.Message}
                 """;
        }

        private static string BuildCareerBody(CareerApplicationViewModel model, IFormFile? resumeFile)
        {
            var resumeNote = resumeFile is { Length: > 0 }
                ? $"Attached ({resumeFile.FileName})"
                : "Not attached";

            return
                $"""
                 New career application from the Shreeyan Webtech website.

                 Name: {model.Name}
                 Email: {model.Email}
                 Phone: {model.Phone ?? "-"}
                 Area of interest: {model.RoleInterest}
                 Experience: {model.Experience ?? "-"}
                 LinkedIn / portfolio / resume link: {model.ProfileUrl ?? "-"}
                 Resume PDF: {resumeNote}

                 Message:
                 {model.Message}
                 """;
        }
    }
}