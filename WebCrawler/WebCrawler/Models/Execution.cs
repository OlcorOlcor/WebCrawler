namespace WebCrawler.Models {
    public class Execution {
        private readonly string _url;
        private readonly string _regex;

        //list of websites to be crawled
        private Queue<string> _queue;

        //list of all sites with their oriented conections
        private List<WebPage> _pages;

        //hashset of already visited sites
        private HashSet<bool> _visited;

        public Execution(string url, string regex) {
            this._url = url;
            this._regex = regex;
            this._queue = new Queue<string> { };
            this._queue.Enqueue(url);
            this._pages = new List<WebPage>();
            this._visited = new HashSet<bool>();
        }

        public void Execute() {
            while (_queue.Count > 0) {
                //TODO
            }
        }        
    }
}
