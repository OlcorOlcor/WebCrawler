namespace WebCrawler.Models {
    public class GraphDataSerializer {
        private WebsiteRecord WebsiteRecord { get; set; }

        public string SerializeRecord(WebsiteRecord record) {
            WebsiteRecord websiteRecord = record;

            foreach(var execution in record.RunningExecutions) {

            }
        }
        private string SerializeExecution() {

        }
        private string SerializeVertices() {

        }
        private string SerializeEdges() {

        }
    }
}
