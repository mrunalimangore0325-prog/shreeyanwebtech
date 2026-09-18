namespace Shreeyan.Models
{
    public class EmailSettings
    {
        /// <summary>Brevo (Sendinblue) transactional API key. Get one free at https://app.brevo.com/settings/keys/api</summary>
        public string BrevoApiKey { get; set; } = string.Empty;

        public string SenderName { get; set; } = "Shreeyan Webtech Website";

        /// <summary>Must be an email/domain verified as a sender in your Brevo account.</summary>
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>Your inbox — where contact form submissions get delivered.</summary>
        public string OwnerEmail { get; set; } = string.Empty;
    }
}