using System.Text;

namespace WebCrawler.Models.Serializers {
    public class GraphDataSerializer : ISerializer<WebsiteRecord> {
        private StringBuilder? sb;
        public string Serialize(WebsiteRecord record) {
            sb = new StringBuilder();
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
            Website[] websites;
            if (execution.websites is not null) {
                websites = execution.websites.ToArray();
            }
            else {
                return;
            }

            sb.Append($"\"{executionNumber}\": {{");
            sb.Append($"\"status\": \"{execution.Status.ToString()}\",");
            sb.Append($"\"start-time\": \"{execution.Start}\",");
            sb.Append($"\"end-time\": \"{execution.End}\",");
            sb.Append($"\"crawled-page-count\": \"{execution.websites.Count}\",");
            SerializeNodes(websites);
            SerializeLinks(websites);
            sb.Append("}");
        }

        private void SerializeNodes(Website[] websites) {
            sb.Append("\"nodes\": [");
            bool firstSite = true;
            foreach (var site in websites) {
                if (!firstSite) {
                    sb.Append(",");
                }
                firstSite = false;

                SerializeNode(
                    site.Url, 
                    site.Title, 
                    site.CrawlTime.ToString(), 
                    new string[] {"https://test.net"}, // TODO Add list of sites that crawled this site
                    2,
                    true
                );

                // TODO maybe could check if url already present
                if (site.OutgoingLinks.UrlsMatchingRegex is not null) {
                    foreach (var link in site.OutgoingLinks.UrlsMatchingRegex) {
                        sb.Append(",");
                        SerializeNode(link, "", "", new string[0], 2, true);
                    }
                }

                if (site.OutgoingLinks.UrlsNotMatchingRegex is not null) {
                    foreach (var link in site.OutgoingLinks.UrlsNotMatchingRegex) {
                        sb.Append(",");
                        SerializeNode(link, "", "", new string[0], 1, false);
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

        private void SerializeLinks(Website[] websites) {
            sb.Append("\"links\": [");
            bool firstSiteToOutput = true;
            foreach (var site in websites) {
                if (site.OutgoingLinks.UrlsMatchingRegex is not null) {
                    foreach (var link in site.OutgoingLinks.UrlsMatchingRegex) {
                        if (!firstSiteToOutput) {
                            sb.Append(",");
                        }
                        firstSiteToOutput = false;
                        sb.Append($"{{\"source\": \"{site.Url}\", \"target\": \"{link}\", \"value\": 1}}");
                    }
                }

                if (site.OutgoingLinks.UrlsNotMatchingRegex is not null) {
                    foreach (var link in site.OutgoingLinks.UrlsNotMatchingRegex) {
                        if (!firstSiteToOutput) {
                            sb.Append(",");
                        }
                        firstSiteToOutput = false;
                        sb.Append($"{{\"source\": \"{site.Url}\", \"target\": \"{link}\", \"value\": 1}}");
                    }
                }
            }
            sb.Append("]");
        }
    }
}
