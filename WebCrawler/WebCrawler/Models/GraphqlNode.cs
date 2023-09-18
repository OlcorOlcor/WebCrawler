namespace WebCrawler.Models {
    public class Node {
        public string? Title { get; set; }
        public string Url { get; set; }
        public string? CrawlTime { get; set; }
        public List<Node>? Links { get; set; }
        public WebPage Owner { get; set; }

        public Node(WebPage owner, string url) {
            Owner = owner;
            Url = url;
        }

        public static Node GetNewNodeWithEmptyLinks(Website website, WebPage owner) {
            return new Node(owner, website.Url) {
                Title = website.Title, 
                CrawlTime = website.CrawlTime.ToString(), 
                Links = new List<Node>()
            };
        }
    }
}
