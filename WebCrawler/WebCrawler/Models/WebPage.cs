namespace WebCrawler.Models {
    public struct WebLinks {
        public string[] UrlsMatchingRegex { get; init; }
        public string[] UrlsNotMatchingRegex { get; init; }
    }

    public struct WebPage {
        public readonly string Url { get; }
        public readonly string Title { get; }
        public readonly WebLinks OutgoingLinks { get; }
        public readonly DateTime CrawlTime { get; }
        public readonly bool Active { get; }
      
        public WebPage(string url, string title, WebLinks OutgoingLinks, DateTime crawlTime, bool active) {
            this.Url = url;
            this.Title = title;
            this.OutgoingLinks = OutgoingLinks;
            this.CrawlTime = crawlTime;
            this.Active = active;
        }
    }
}   
