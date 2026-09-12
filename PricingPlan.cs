namespace Shreeyan.Models
{
    public class PricingPlan
    {
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
        public bool Highlighted { get; set; }
        public string CtaText { get; set; } = string.Empty;
    }
}