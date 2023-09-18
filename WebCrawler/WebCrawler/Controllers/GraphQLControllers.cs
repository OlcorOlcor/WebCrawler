using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using WebCrawler.Models;

namespace WebCrawler.Controllers {
    public class WebsitesController : GraphController {
        private readonly ILogger<ApiController> _logger;
        protected WebsiteRecordRepository? repo;
        public WebsitesController(ILogger<ApiController> logger, WebsiteRecordRepository repo) {
            this._logger = logger;
            this.repo = repo;
        }

        [QueryRoot]
        public List<WebPage> Websites() {
            var records = repo!.GetAllRecords();
            List<WebPage> websites = new List<WebPage>();

            foreach (var record in records) {
                websites.Add(WebPage.MakeNewWebPage(record));
            }

            return websites;
        }
    }

    public class NodesController : GraphController {
        private readonly ILogger<ApiController> _logger;
        protected WebsiteRecordRepository? repo;
        public NodesController(ILogger<ApiController> logger, WebsiteRecordRepository repo) {
            this._logger = logger;
            this.repo = repo;
        }

        [QueryRoot]
        public List<Node> Nodes(int[] webPages) {
            var records = new List<WebsiteRecord>();

            foreach (var webPageId in webPages) {
                var record = repo!.Find(webPageId);
                if (record is null) {
                    throw new ArgumentException();
                }

                records.Add(record);
            }

            List<WebPage> pages = new List<WebPage>();
            foreach (var record in records) {
                pages.Add(WebPage.MakeNewWebPage(record));
            }

            List<Node> nodes = new List<Node>(); 
            int index = 0;
            foreach (var record in records) {
                List<Website> sites = new List<Website>();
                if (record.LastFinishedExecution is not null) {
                    sites = record.LastFinishedExecution.websites;
                }
                else if (record.RunningExecutions.Count > 0) {
                    sites = record.RunningExecutions[0].websites; 
                }
                else {
                    index++;
                    continue;
                }

                nodes.AddRange(MakeNodesFromRecordSites(sites, pages[index]));

                index++;
            }
            return nodes;
        }

        private static List<Node> MakeNodesFromRecordSites(List<Website> sites, WebPage owner) {
            var foundNodes = new Dictionary<string, Node>();

            foreach (var site in sites) {
                if (!foundNodes.ContainsKey(site.Url)) {
                    foundNodes.Add(site.Url, Node.GetNewNodeWithEmptyLinks(site, owner));
                }
                else {
                    CopyInfoToNode(Node.GetNewNodeWithEmptyLinks(site, owner), foundNodes[site.Url]);
                }
                
                Node siteNode = foundNodes[site.Url];
                foreach (var link in site.OutgoingLinks.UrlsMatchingRegex) {
                    if (foundNodes.TryGetValue(link, out Node? linkNode)) {
                        siteNode!.Links.Add(linkNode!);
                    }
                    else {
                        Node newNode = new Node(owner, link);
                        foundNodes.Add(link, newNode);
                        siteNode!.Links.Add(newNode);
                    }
                }
            }

            var outputNodes = new List<Node>();
            foreach (var foundNodePair in foundNodes) {
                outputNodes.Add(foundNodePair.Value);
            }

            return outputNodes;
        }

        private static void CopyInfoToNode(Node copyFrom, Node copyTo) {
            copyTo.Title = copyFrom.Title;
            copyTo.CrawlTime = copyFrom.CrawlTime;
        }
    }
}