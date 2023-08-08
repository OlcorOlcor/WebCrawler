namespace WebCrawler.Models {
    public struct WebPage {
        public readonly string Url;
        public readonly string? Title; 
        public readonly string[] OutgoingUrls;
        public readonly DateTime? CrawlTime;
       
        public WebPage(string url, string title, string[] OutgoingUrls, DateTime crawlTime) {
            this.Url = url;
            this.Title = title;
            this.OutgoingUrls = OutgoingUrls;
            this.CrawlTime = crawlTime;
        }
    }
}   
