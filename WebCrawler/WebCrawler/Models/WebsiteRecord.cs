namespace WebCrawler.Models {
    public class WebsiteRecord {
        public int Id { get; init; } 
        public string Url { get; set; }
        public string Regex { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public string Label { get; set; }
    }
}
