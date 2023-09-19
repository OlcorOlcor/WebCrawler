namespace WebCrawler.Models {
    public class Website {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
        public string Regex { get; set; }
        public string[] Tags { get; set; }
        public bool Active { get; set; }

        public Website(int id, string label, string url, string regex, string[] tags, bool active) {
            Id = id;
            Label = label;
            Url = url;
            Regex = regex;
            Tags = tags; 
            Active = active;
        }

        public static Website MakeNewWebsite(WebsiteRecord record) {
            return new Website(record.Id, record.Label, record.Url, record.Regex, record.TagsArray, record.Active);
        }
    }
}