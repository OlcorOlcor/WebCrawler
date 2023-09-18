namespace WebCrawler.Models {
    public class Node {
        public string? Title { get; set; }
        public string Url { get; set; }
        public string? CrawlTime { get; set; }
        public List<Node>? Links { get; set; }
        public Website Owner { get; set; }

        public Node(Website owner, string url) {
            Owner = owner;
            Url = url;
        }
    }
}
