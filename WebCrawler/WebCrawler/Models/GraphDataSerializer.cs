using System;
using System.Text;

namespace WebCrawler.Models {
    public class GraphDataSerializer {
        private StringBuilder sb = new StringBuilder();
        public string SerializeRecord(WebsiteRecord record) {
            WebsiteRecord websiteRecord = record;
            sb.Append("{");
            sb.Append("\"executions\": {");

            int executionNumber = 0;
            int executionCount = record.RunningExecutions.Count;
            bool lastFinishedExecution = false;
            if (websiteRecord.LastFinishedExecution is not null) {
                SerializeExecution(websiteRecord.LastFinishedExecution, executionNumber++);
                if (executionCount > 0) {
                    sb.Append(",");
                }
                
                lastFinishedExecution = true;
            }

            int forLastFinishedExecution = lastFinishedExecution ? 1 : 0;
            foreach (var execution in record.RunningExecutions) {
                SerializeExecution(execution, executionNumber);
                if (executionNumber < executionCount - 1 + forLastFinishedExecution) {
                    sb.Append(",");
                }
                executionNumber++;
            }

            sb.Append("}}");
            return sb.ToString();
        }
        private void SerializeExecution(Execution execution, int executionNumber) {
            WebPage[] webPages;
            if (execution.pages is not null) {
                webPages = execution.pages.ToArray();
            }
            else {
                return;
            }
            
            sb.Append($"\"{executionNumber}\": {{");
            SerializeNodes(webPages);
            SerializeLinks(webPages);
            sb.Append("}");
        }
        private void SerializeNodes(WebPage[] webPages) {
            sb.Append("\"nodes\": [");
            var pageCount = webPages.Length;
            int runningCount = 0;
            foreach (var page in webPages) {
                sb.Append($"{{\"id\": \"{page.Url}\", \"group\": 1}}");
                if (runningCount < pageCount - 1) {
                    sb.Append(",");
                }
                runningCount++;
            }
            sb.Append("],");
        }
        private void SerializeLinks(WebPage[] webPages) {
            sb.Append("\"links\": [");
            bool firstPageToOutput = true;
            foreach (var webPage in webPages) {
                var outgoingUrlCount = webPage.OutgoingUrls.Length;
                int runningUrlCount = 0;

                if (!firstPageToOutput && outgoingUrlCount != 0) {
                    sb.Append(",");
                }
                firstPageToOutput = false;

                foreach (var link in webPage.OutgoingUrls) {
                    sb.Append($"{{\"source\": \"{webPage.Url}\", \"target\": \"{link}\", \"value\": 1}}");
                    if (runningUrlCount < outgoingUrlCount - 1) {
                        sb.Append(",");
                    }
                    runningUrlCount++;
                }
            }
            sb.Append("]");
        }
    }
}
