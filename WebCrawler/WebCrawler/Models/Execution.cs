namespace WebCrawler.Models {
    public class Execution {
        private readonly string _url;
        private readonly string _regex;

        //list of websites to be crawled
        private Queue<WebPage> _queue;

        //list of all sites with their oriented conections
        private List<WebPage> _pages;

        //hashset of already visited sites
        private HashSet<bool> _visited;

        //Crawler for crawling current website
        private Crawler _crawler = new();
        public Execution(string url, string regex) {
            this._url = url;
            this._regex = regex;
            this._queue = new Queue<WebPage>();
            this._queue.Enqueue(new WebPage(url));
            this._pages = new List<WebPage>();
            this._visited = new HashSet<bool>();
        }

        //does all the crawling
        public void Execute() {
            while (_queue.Count > 0) {
                var page = _queue.Dequeue();
                _crawler.CrawlSite(page, _regex);
            }
        }        
    }
}
