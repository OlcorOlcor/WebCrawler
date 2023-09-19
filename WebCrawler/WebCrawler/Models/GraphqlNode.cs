namespace WebCrawler.Models {
    public class Node {
        public string? Title { get; set; }
        public string Url { get; set; }
        public string? CrawlTime { get; set; }
        public List<string>? Links { get; set; }
        public Website Owner { get; set; }

        public Node(Website owner, string url) {
            Owner = owner;
            Url = url;
        }

        public static Node GetNewNode(WebPage webPage, Website owner) {
            return new Node(owner, webPage.Url) {
                Title = webPage.Title, 
                CrawlTime = webPage.CrawlTime.ToString(), 
                Links = webPage.OutgoingLinks.UrlsMatchingRegex.ToList()
            };
        }
    }
}