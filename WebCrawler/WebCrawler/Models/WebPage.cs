namespace WebCrawler.Models {
    public class WebPage {
        public string Url;
        public string? Title; 
        public List<string> OutgoingUrls = new();
        public DateTime? CrawlTime;
        public WebPage(string url) {
            this.Url = url;
        }
        public WebPage(string url, string title, List<string> OutgoingUrls, DateTime crawlTime) {
            this.Url = url;
            this.Title = title;
            this.OutgoingUrls = OutgoingUrls;
            this.CrawlTime = crawlTime;
        }
    }
}   
