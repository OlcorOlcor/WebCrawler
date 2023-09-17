namespace WebCrawler.Models {
    public struct WebLinks {
        public string[] UrlsMatchingRegex { get; init; }
        public string[] UrlsNotMatchingRegex { get; init; }
    }

    public struct WebPage {
        static int _Id = 0;
        public readonly int Id { get; }
        public readonly string Url { get; }
        public readonly string Title { get; }
        public readonly WebLinks OutgoingLinks { get; }
        public readonly DateTime CrawlTime { get; }
        public readonly bool Active { get; }
      
        public WebPage(string url, string title, WebLinks OutgoingLinks, DateTime crawlTime, bool active) {
            WebPage._Id++;
            this.Id = WebPage._Id;
            this.Url = url;
            this.Title = title;
            this.OutgoingLinks = OutgoingLinks;
            this.CrawlTime = crawlTime;
            this.Active = active;
        }
    }
}   
