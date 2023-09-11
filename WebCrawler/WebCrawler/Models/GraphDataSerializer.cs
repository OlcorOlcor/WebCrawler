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
                sb.Append($"{{\"id\": \"{page.Url}\", \"group\": 2, \"match\": \"true\"}}");

                // TODO maybe could check if url already present
                if (page.OutgoingLinks.UrlsMatchingRegex is not null) {
                    foreach (var link in page.OutgoingLinks.UrlsMatchingRegex) {
                        sb.Append($",{{\"id\": \"{link}\", \"group\": 2, \"match\": \"true\"}}");
                    }
                }
                
                if (page.OutgoingLinks.UrlsNotMatchingRegex is not null) {
                    foreach (var link in page.OutgoingLinks.UrlsNotMatchingRegex) {
                        sb.Append($",{{\"id\": \"{link}\", \"group\": 1, \"match\": \"false\"}}");
                    }
                }
            }
            sb.Append("],");
        }
        private void SerializeLinks(WebPage[] webPages) {
            sb.Append("\"links\": [");
            bool firstPageToOutput = true;
            foreach (var page in webPages) {
                if (page.OutgoingLinks.UrlsMatchingRegex is not null) {
                    foreach (var link in page.OutgoingLinks.UrlsMatchingRegex) {
                        if (!firstPageToOutput) {
                            sb.Append(",");
                        }
                        firstPageToOutput = false;
                        sb.Append($"{{\"source\": \"{page.Url}\", \"target\": \"{link}\", \"value\": 1}}");
                    }
                }

                if (page.OutgoingLinks.UrlsMatchingRegex is not null) {
                    foreach (var link in page.OutgoingLinks.UrlsNotMatchingRegex) {
                        if (!firstPageToOutput) {
                            sb.Append(",");
                        }
                        firstPageToOutput = false;
                        sb.Append($"{{\"source\": \"{page.Url}\", \"target\": \"{link}\", \"value\": 1}}");
                    }
                }
            }
            sb.Append("]");
        }
    }
}
