using System.Diagnostics;

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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (_queue.Count > 0) {
                var page = _queue.Dequeue();
                CrawlResponse response;
                try {
                    response = await _crawler.CrawlSite(page, _regex);
                }
                catch (Exception e){
                    Console.WriteLine(e.Message);
                    continue;
                }
                
                WebPage updatedPage = new WebPage(page, response.PageTitle, response.Links, DateTime.Now);

                foreach (var outgoingLink in updatedPage.OutgoingLinks.UrlsMatchingRegex) { 
                    if(!_visited.Contains(outgoingLink)) {
                        _visited.Add(outgoingLink);
                        _queue.Enqueue(outgoingLink);
                    }
                }
                pages.Add(updatedPage);
            }
          
            stopwatch.Stop();
            this.ExecutionTime = stopwatch.Elapsed;
          
            if (updateRepositoryCallback is not null) {
                updateRepositoryCallback.Invoke(this);
            }
        }
    }
}
