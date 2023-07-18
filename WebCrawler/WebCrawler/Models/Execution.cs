namespace WebCrawler.Models {
    public class Execution {
        private readonly string _url;
        private readonly string _regex;

        public Execution(string url, string regex) {
            this._url = url;
            this._regex = regex;
        }

        public void Execute() {
            //TODO
        }

        // list of pages and pages to be crawled
    }
}
