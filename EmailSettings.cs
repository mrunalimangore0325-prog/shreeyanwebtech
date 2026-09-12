namespace Shreeyan.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SenderName { get; set; } = "Shreeyan Webtech Website";
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;

        /// <summary>Your inbox — where contact form submissions get delivered.</summary>
        public string OwnerEmail { get; set; } = string.Empty;

        public bool EnableSsl { get; set; } = true;
    }
}