
namespace WebCrawler.Models {
   
    public enum Status { NotRunning, Running, Finished }
    
    public class Execution {
        public readonly string _url;
        private readonly string _regex;

        public Status Status { get; set; } = Status.NotRunning;
        public DateTime? Start { get; set; } = null;
        public DateTime? End { get; set; } = null;

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
        //TODO this is not filled anywhere but used in one serialization
        public TimeSpan ExecutionTime;

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

            Start = DateTime.Now;
            Status = Status.Running;

            while (_queue.Count > 0) {
                var page = _queue.Dequeue();

                WebPage crawledPage = await _crawler.CrawlSite(page, _regex);
                pages.Add(crawledPage);

                if (crawledPage.OutgoingLinks.UrlsMatchingRegex is null) {
                    continue;
                }

                foreach (var outgoingLink in crawledPage.OutgoingLinks.UrlsMatchingRegex) { 
                    if(!_visited.Contains(outgoingLink)) {
                        _visited.Add(outgoingLink);
                        _queue.Enqueue(outgoingLink);
                    }
                }
            }

            End = DateTime.Now;
            Status = Status.Finished;

            if (updateRepositoryCallback is not null) {
                updateRepositoryCallback.Invoke(this);
            }
        }
    }
}
