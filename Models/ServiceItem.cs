namespace Shreeyan.Models
{
    public class ServiceItem
    {
        public string IconKey { get; set; } = "code";
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Includes { get; set; } = new();
    }
}