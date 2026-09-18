namespace Shreeyan.Models
{
    public class Company
    {
        public string Name { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;
        public string IconKey { get; set; } = "building";

        /// <summary>Shown on the back of the flip card.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Short bullet points shown on the back of the flip card.</summary>
        public List<string> Highlights { get; set; } = new();

        /// <summary>e.g. "Since 2019" — optional, shown on the back if set.</summary>
        public string? Established { get; set; }

        // Leave Url null/empty until the company's own website is live.
        // The Companies page automatically shows a "Visit website" link once this is set,
        // and a "Coming soon" badge while it's null.
        public string? Url { get; set; }
    }
}