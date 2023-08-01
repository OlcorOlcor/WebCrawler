namespace WebCrawler.Models {
    public class WebPageExecution {
        public WebPage WebPage { get; set; }
        private string _regex { get; set; }
        public delegate void UpdateUponCompletion(List<WebPage> foundWebPages);
        //Crawler for crawling
        private Crawler _crawler = new();
        public WebPageExecution(WebPage webPage, string regex) {
            WebPage = webPage;
            _regex = regex;
        }
        public void Execute() {
            _crawler.CrawlSite(WebPage, _regex);
        }
    }
}
