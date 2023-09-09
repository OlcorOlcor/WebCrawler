namespace WebCrawler.Models {
    public struct WebLinks {
        public string[] UrlsMatchingRegex { get; init; }
        public string[] UrlsNotMatchingRegex { get; init; }
    }

    public struct WebPage {
        public readonly string Url;
        public readonly string? Title; 
        public readonly WebLinks OutgoingLinks;
        public readonly DateTime? CrawlTime;
       
        public WebPage(string url, string title, WebLinks OutgoingLinks, DateTime crawlTime) {
            this.Url = url;
            this.Title = title;
            this.OutgoingLinks = OutgoingLinks;
            this.CrawlTime = crawlTime;
        }
    }
}   
