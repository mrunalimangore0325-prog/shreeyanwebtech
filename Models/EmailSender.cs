using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shreeyan.Models;

namespace Shreeyan.Services
{
    public class EmailSender : IEmailSender
    {
        private const string BrevoSendEndpoint = "https://api.brevo.com/v3/smtp/email";

        private readonly EmailSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> settings, HttpClient httpClient, ILogger<EmailSender> logger)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendContactMessageAsync(ContactViewModel model)
        {
            var payload = new BrevoEmailRequest
            {
                Sender = new BrevoContact { Email = _settings.SenderEmail, Name = _settings.SenderName },
                To = new[] { new BrevoContact { Email = _settings.OwnerEmail } },
                ReplyTo = !string.IsNullOrWhiteSpace(model.Email)
                    ? new BrevoContact { Email = model.Email, Name = model.Name }
                    : null,
                Subject = $"New enquiry — {model.ProjectType} ({model.Name})",
                TextContent = BuildContactBody(model)
            };

            await SendViaBrevoAsync(payload, $"contact email for {model.Email}");
        }

        public async Task SendCareerApplicationAsync(CareerApplicationViewModel model, IFormFile? resumeFile)
        {
            var payload = new BrevoEmailRequest
            {
                Sender = new BrevoContact { Email = _settings.SenderEmail, Name = _settings.SenderName },
                To = new[] { new BrevoContact { Email = _settings.OwnerEmail } },
                ReplyTo = !string.IsNullOrWhiteSpace(model.Email)
                    ? new BrevoContact { Email = model.Email, Name = model.Name }
                    : null,
                Subject = $"New career application — {model.RoleInterest} ({model.Name})",
                TextContent = BuildCareerBody(model, resumeFile)
            };

            // Brevo expects attachments as base64 content, not a raw stream.
            if (resumeFile is { Length: > 0 })
            {
                await using var stream = resumeFile.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                payload.Attachment = new[]
                {
                    new BrevoAttachment
                    {
                        Name = resumeFile.FileName,
                        Content = Convert.ToBase64String(memoryStream.ToArray())
                    }
                };
            }

            await SendViaBrevoAsync(payload, $"career application email for {model.Email}");
        }

        private async Task SendViaBrevoAsync(BrevoEmailRequest payload, string logContext)
        {
            if (string.IsNullOrWhiteSpace(_settings.BrevoApiKey))
            {
                _logger.LogError("Brevo API key is not configured. Cannot send {LogContext}.", logContext);
                throw new InvalidOperationException("Email sending is not configured (missing Brevo API key).");
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, BrevoSendEndpoint)
            {
                Content = JsonContent.Create(payload, options: JsonOptions)
            };
            request.Headers.Add("api-key", _settings.BrevoApiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogError(
                        "Brevo API returned {StatusCode} while sending {LogContext}. Response: {Body}",
                        response.StatusCode, logContext, body);
                    throw new InvalidOperationException($"Brevo API request failed with status {response.StatusCode}.");
                }

                _logger.LogInformation("Sent {LogContext} via Brevo.", logContext);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while sending {LogContext} via Brevo.", logContext);
                throw;
            }
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

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

        // ---- Brevo API request DTOs ----

        private class BrevoEmailRequest
        {
            [JsonPropertyName("sender")]
            public BrevoContact Sender { get; set; } = new();

            [JsonPropertyName("to")]
            public BrevoContact[] To { get; set; } = Array.Empty<BrevoContact>();

            [JsonPropertyName("replyTo")]
            public BrevoContact? ReplyTo { get; set; }

            [JsonPropertyName("subject")]
            public string Subject { get; set; } = string.Empty;

            [JsonPropertyName("textContent")]
            public string TextContent { get; set; } = string.Empty;

            [JsonPropertyName("attachment")]
            public BrevoAttachment[]? Attachment { get; set; }
        }

        private class BrevoContact
        {
            [JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;

            [JsonPropertyName("name")]
            public string? Name { get; set; }
        }

        private class BrevoAttachment
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            /// <summary>Base64-encoded file content.</summary>
            [JsonPropertyName("content")]
            public string Content { get; set; } = string.Empty;
        }
    }
}