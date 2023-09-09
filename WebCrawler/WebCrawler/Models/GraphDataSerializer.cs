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
            bool firstExecution = true;
            if (websiteRecord.LastFinishedExecution is not null) {
                SerializeExecution(websiteRecord.LastFinishedExecution, executionNumber++);                
                firstExecution = false;
            }

            foreach (var execution in record.RunningExecutions) {
                if (!firstExecution) {
                    sb.Append(",");
                }
                firstExecution = false;
                SerializeExecution(execution, executionNumber);
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

            sb.Append("\"ExecutionNumber\" : ");
            sb.Append($"\"{executionNumber}\",");
            sb.Append("\"Status\" : ");
            sb.Append($"\"{execution.Status.ToString()}\",");
            sb.Append("\"Start\" : ");
            sb.Append($"\"{execution.Start}\",");
            sb.Append("\"End\" : ");
            sb.Append($"\"{execution.End}\",");
            sb.Append("\"NumberOfCrawledPages\" : ");
            sb.Append($"\"{execution.pages.Count}\",");
            sb.Append("\"Nodes\" : ");
            SerializeNodes(webPages);
            sb.Append("\"Links\" : ");
            SerializeLinks(webPages);
            sb.Append("}");
        }
        private void SerializeNodes(WebPage[] webPages) {
            sb.Append("\"nodes\": [");
            bool firstPage = true;
            foreach (var page in webPages) {
                if (!firstPage) {
                    sb.Append(",");
                }
                firstPage = false;
                sb.Append($"{{\"id\": \"{page.Url}\", \"group\": 1}}");

                // TODO maybe could check if url already present
                foreach (var link in page.OutgoingUrls) {
                    sb.Append($",{{\"id\": \"{link}\", \"group\": 1}}");
                }
            }
            sb.Append("],");
        }
        private void SerializeLinks(WebPage[] webPages) {
            sb.Append("\"links\": [");
            bool firstPageToOutput = true;
            foreach (var page in webPages) {
                var outgoingUrlCount = page.OutgoingUrls.Length;
                int runningOutgoingUrlCount = 0;

                if (!firstPageToOutput && outgoingUrlCount != 0) {
                    sb.Append(",");
                }
                firstPageToOutput = false;

                foreach (var link in page.OutgoingUrls) {
                    sb.Append($"{{\"source\": \"{page.Url}\", \"target\": \"{link}\", \"value\": 1}}");
                    if (runningOutgoingUrlCount < outgoingUrlCount - 1) {
                        sb.Append(",");
                    }
                    runningOutgoingUrlCount++;
                }
            }
            sb.Append("]");
        }
    }
}
