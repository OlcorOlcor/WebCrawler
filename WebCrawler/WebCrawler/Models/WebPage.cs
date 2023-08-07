namespace WebCrawler.Models {
    public struct WebPage {
        public string Url;
        public string? Title; 
        public string[] OutgoingUrls;
        public DateTime? CrawlTime;
       
        public WebPage(string url, string title, string[] OutgoingUrls, DateTime crawlTime) {
            this.Url = url;
            this.Title = title;
            this.OutgoingUrls = OutgoingUrls;
            this.CrawlTime = crawlTime;
        }
    }
}   
