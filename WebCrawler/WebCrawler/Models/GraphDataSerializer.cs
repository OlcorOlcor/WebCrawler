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

            sb.Append($"\"{executionNumber}\": {{");
            sb.Append($"\"status\": \"{execution.Status.ToString()}\",");
            sb.Append($"\"start-time\": \"{execution.Start}\",");
            sb.Append($"\"end-time\": \"{execution.End}\",");
            sb.Append($"\"crawled-page-count\": \"{execution.pages.Count}\",");
            SerializeNodes(webPages);
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
                SerializeNode(
                    page.Url, 
                    page.Title, 
                    page.CrawlTime.ToString(), 
                    new string[] {"https://test.net"}, // TODO Add list of sites that crawled this site
                    1
                );

                // TODO maybe could check if url already present
                foreach (var link in page.OutgoingUrls) {
                    sb.Append(",");
                    SerializeNode(link, "", "", new string[0], 1);
                }
            }
            sb.Append("],");
        }

        private void SerializeNode(string id, string title, string crawlTime, string[] crawledBy, int group) {
            sb.Append($"{{\"id\": \"{id}\"");
            sb.Append($",\"title\": \"{title}\"");
            sb.Append($",\"crawl-time\": \"{crawlTime}\"");
            sb.Append($",\"crawled-by\": [");
            bool firstUrl = true;
            foreach (string url in crawledBy) {
                if (!firstUrl) {
                    sb.Append(",");
                }
                firstUrl = false;
                sb.Append($"\"{url}\"");
            }
            sb.Append($"]");
            sb.Append($",\"group\": {group}}}");
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
