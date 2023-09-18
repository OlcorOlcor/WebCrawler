using System.Text;
using System.Text.RegularExpressions;

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

        public string SerializeById(List<int> listId, WebsiteRecordRepository repo) {
            List<(WebPage,string, int)> allPages = new List<(WebPage,string,int)>();
            List<WebPage> links = new List<WebPage> ();
            var records = repo!.GetAll();
            foreach (int id in listId) {
                foreach (var record in records) {
                    if (record.Id == id) {
                        List<WebPage> pages;
                        if (record.RunningExecutions.Count != 0) {
                            pages = record.RunningExecutions[0].pages;
                        }
                        else if (record.LastFinishedExecution != null) {
                            pages = record.LastFinishedExecution.pages;
                        }
                        else continue;

                        foreach(var page in pages) {
                            allPages.Add((page, record.Label!, record.Id!));
                        }
                        links.AddRange(pages);
                    }
                }
            }
            sb = new StringBuilder();
            sb.Append("\"graph data\": {");
            SerializeRepeatingNodes(allPages);
            SerializeLinks(links.ToArray());
            sb.Append("}");
            return sb.ToString();
        }

        private void SerializeRepeatingNodes(List<(WebPage,string, int)> allPages) {
            List<(WebPage,string, int)> allPagesSorted = allPages.OrderBy(o => o.Item1.Url).ToList();
            string lastUrl = allPages[0].Item1.Url;
            DateTime? mostRecentTime = null;
            List<(string,int)> crawledBy = new List<(string, int)>();

            sb.Append("\"nodes\": [");

            sb.Append($"{{\"id\": \"{allPages[0].Item1.Url}\"");
            sb.Append($",\"title\": \"{allPages[0].Item1.Title}\"");
            sb.Append($",\"group\": 2");
            sb.Append($",\"match\": \"true\"");

            foreach (var page in allPagesSorted) {
                if(page.Item1.Url == lastUrl ) {
                    if(mostRecentTime > page.Item1.CrawlTime || mostRecentTime == null) {
                        mostRecentTime = page.Item1.CrawlTime;
                    }
                    crawledBy.Add((page.Item2,page.Item3));
                }
                else {
                    sb.Append($",\"crawl-time\": \"{mostRecentTime}\"");
                    sb.Append($",\"crawled-by\": [");
                    bool firstUrl = true;
                    foreach ((string, int) url in crawledBy) {
                        if (!firstUrl) {
                            sb.Append(",");
                        }
                        firstUrl = false;
                        sb.Append($"{url.Item2} : \"{url.Item1}\"");
                    }
                    sb.Append($"}}");

                    mostRecentTime = null;
                    crawledBy = new List<(string, int)>();

                    if (mostRecentTime > page.Item1.CrawlTime || mostRecentTime == null) {
                        mostRecentTime = page.Item1.CrawlTime;
                    }
                    crawledBy.Add((page.Item2, page.Item3));

                    sb.Append($"{{\"id\": \"{page.Item1.Url}\"");
                    sb.Append($",\"title\": \"{page.Item1.Title}\"");
                    sb.Append($",\"group\": 2");
                    sb.Append($",\"match\": \"true\"");
                }
            }

            if(mostRecentTime != null) {
                sb.Append($",\"crawl-time\": \"{mostRecentTime}\"");
                sb.Append($",\"crawled-by\": [");
                bool firstUrl = true;
                foreach ((string, int) url in crawledBy) {
                    if (!firstUrl) {
                        sb.Append(",");
                    }
                    firstUrl = false;
                    sb.Append($"{url.Item2} : \"{url.Item1}\"");
                }
                sb.Append($"}}");
            }

            sb.Append("],");
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
                    2,
                    true
                );

                // TODO maybe could check if url already present
                if (page.OutgoingLinks.UrlsMatchingRegex is not null) {
                    foreach (var link in page.OutgoingLinks.UrlsMatchingRegex) {
                        sb.Append(",");
                        SerializeNode(link, "", "", new string[0], 2, true);
                    }
                }

                if (page.OutgoingLinks.UrlsNotMatchingRegex is not null) {
                    foreach (var link in page.OutgoingLinks.UrlsNotMatchingRegex) {
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
