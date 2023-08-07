using System;
using System.Text;

namespace WebCrawler.Models {
    public class GraphDataSerializer {
        private WebsiteRecord _websiteRecord { get; set; }
        private StringBuilder sb = new StringBuilder();
        public string SerializeRecord(WebsiteRecord record) {
            WebsiteRecord websiteRecord = record;
            sb.Append("{[");
            int executionNumber = 0;

            SerializeExecution(websiteRecord.LastFinishedExecution, executionNumber++);
            foreach (var execution in record.RunningExecutions) {
                SerializeExecution(execution, executionNumber);
                executionNumber++;
            }
            sb.Append("]}");
            return sb.ToString();
        }
        private void SerializeExecution(Execution execution, int executionNumber) {
            WebPage[] webPages = execution.pages.ToArray();
            sb.Append($"\"Execution{executionNumber}\": {{ ");
            SerializeNodes(webPages);
            SerializeLinks(webPages);
        }
        private void SerializeNodes(WebPage[] webPages) {
            sb.Append("\"nodes\": [");
            foreach (var page in webPages) {
                sb.Append($"{{\"id\": {page.Url}, \"group\": 1}},");
            }
            sb.Append("],");
        }
        private void SerializeLinks(WebPage[] webPages) {
            sb.Append("\"links\": [");
            foreach (var webPage in webPages) {
                foreach (var link in webPage.OutgoingUrls) {
                    sb.Append($"{{\"source\": {webPage.Url}, \"target\": {link}, \"value\": 1}},");
                }
            }
            sb.Append("]");
        }
    }
}
