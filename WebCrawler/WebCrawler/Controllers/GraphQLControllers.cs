using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
        public List<Website> Nodes(int[] recordIds) {
            var records = new List<WebsiteRecord>();

            foreach (var recordId in recordIds) {
                var record = repo!.Find(recordId);
                if (record is null) {
                    throw new ArgumentException();
                }

                records.Add(record);
            }

            List<WebPage> pages = new List<WebPage>();
            foreach (var record in records) {
                pages.Add(WebPage.MakeNewWebPage(record));
            }

            List<Website> sites = new List<Website>();
            foreach (var record in records) {
                if (record.LastFinishedExecution is not null) {
                    sites.AddRange(record.LastFinishedExecution.websites);
                }
            }
            return sites;
        }
    }
}