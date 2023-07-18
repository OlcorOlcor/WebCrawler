namespace WebCrawler.Models {
    public class WebPage {
        public string Url;
        public string Title;
        public List<string> OutgoingUrls;
        public TimeSpan CrawlTime;

        public WebPage(string url, string title, List<string> OutgoingUrls, TimeSpan crawlTime) {
            this.Url = url;
            this.Title = title;
            this.OutgoingUrls = OutgoingUrls;
            this.CrawlTime = crawlTime;
        }
    }
}
