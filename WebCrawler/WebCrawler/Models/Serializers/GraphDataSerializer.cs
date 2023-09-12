using System.Text;

namespace WebCrawler.Models.Serializers {
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
                    new string[] { "https://test.net" }, // TODO Add list of sites that crawled this site
                    1,
                    true
                );

                // TODO maybe could check if url already present
                if (page.OutgoingLinks.UrlsMatchingRegex is not null) {
                    foreach (var link in page.OutgoingLinks.UrlsMatchingRegex) {
                        sb.Append(",");
                        SerializeNode(link, "", "", new string[0], 1, true);
                    }
                }

                if (page.OutgoingLinks.UrlsNotMatchingRegex is not null) {
                    foreach (var link in page.OutgoingLinks.UrlsNotMatchingRegex) {
                        sb.Append(",");
                        SerializeNode(link, "", "", new string[0], 2, false);
                    }
                }
            }
            sb.Append("],");
        }

        private void SerializeNode(string id, string title, string crawlTime, string[] crawledBy, int group, bool match) {
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
            sb.Append($",\"group\": {group}");
            sb.Append($",\"match\": \"{match.ToString().ToLower()}\"}}");
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

                if (page.OutgoingLinks.UrlsNotMatchingRegex is not null) {
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
