using WebCrawler.Models;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebCrawler.Controllers {
    public class WebsitesController : GraphController {
        private readonly ILogger<ApiController> _logger;
        protected WebsiteRecordRepository? repo;
        public WebsitesController(ILogger<ApiController> logger, WebsiteRecordRepository repo) {
            this._logger = logger;
            this.repo = repo;
        }

        [QueryRoot]
        public List<Website> Websites() {
            var records = repo!.GetAllRecords();
            List<Website> websites = new List<Website>();

            foreach (var record in records) {
                websites.Add(Website.MakeNewWebsite(record));
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
        public List<Node> Nodes(int[] websites) {
            var records = new List<WebsiteRecord>();

            foreach (var recordId in websites) {
                var record = repo!.Find(recordId);
                if (record is null) {
                    throw new ArgumentException();
                }

                records.Add(record);
            }

            List<Website> sites = new List<Website>();
            foreach (var record in records) {
                sites.Add(Website.MakeNewWebsite(record));
            }

            List<Node> nodes = new List<Node>();
            int index = 0;
            foreach (var record in records) {
                if (record.LastFinishedExecution is not null) {
                    foreach (var page in record.LastFinishedExecution.pages) {
                        nodes.Add(Node.GetNewNode(page, sites[index]));
                    }
                }
                else if (record.RunningExecutions.Count > 0) {
                    foreach (var page in record.RunningExecutions[0].pages) {
                        nodes.Add(Node.GetNewNode(page, sites[index]));
                    }
                }

                index++;
            }

            return nodes;
        }
    }
}