namespace WebCrawler.Models {
    public class Execution {
        public readonly string _url;
        private readonly string _regex;

        //Delegate that leads to WebsiteRecordRepository and updates Manager
        public delegate void UpdateRepository(Execution execution);
        public UpdateRepository? updateRepositoryCallback;

        //list of urls to be crawled
        private Queue<string> _queue;

        //list of all crawled sites with their oriented conections
        public List<WebPage> pages;

        //hashset of already visited urls
        private HashSet<string> _visited;

        //Crawler for crawling current website
        private Crawler _crawler = new();
        public Execution(string url, string regex) {
            this._url = url;
            this._regex = regex;
            this._queue = new Queue<string>();
            this._queue.Enqueue(url);
            this.pages = new List<WebPage>();
            this._visited = new HashSet<string>();
        }

        //does all the crawling
        public async void Execute(object? state) {
            while (_queue.Count > 0) {
                var page = _queue.Dequeue();
                string[] foundPages = await _crawler.CrawlSite(page, _regex);
                WebPage updatedPage = new WebPage(page, _regex, foundPages, DateTime.Now);
                foreach (var outgoingUrl in updatedPage.OutgoingUrls) { 
                    if(!_visited.Contains(outgoingUrl)) {
                        _visited.Add(outgoingUrl);
                        _queue.Enqueue(outgoingUrl);
                    }
                }
                pages.Add(updatedPage);
            }
            Console.WriteLine("loop ended");
            if (updateRepositoryCallback is not null) {
                updateRepositoryCallback.Invoke(this);
            }
        }
    }
}
