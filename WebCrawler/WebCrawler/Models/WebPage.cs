namespace WebCrawler.Models {
    public struct WebPage {
        public readonly string Url;
        public readonly string Title; 
        public readonly string[] OutgoingUrls;
        public readonly DateTime CrawlTime;
        public readonly bool Active;
       
        public WebPage(string url, string title, string[] OutgoingUrls, DateTime crawlTime, bool active) {
            this.Url = url;
            this.Title = title;
            this.OutgoingUrls = OutgoingUrls;
            this.CrawlTime = crawlTime;
            this.Active = active;
        }
    }
}   
