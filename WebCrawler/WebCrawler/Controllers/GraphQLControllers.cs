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
        public List<WebsiteRecord> Websites() {
            return repo!.GetAllRecords();
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
        public List<WebPage> Nodes() {
            var records = repo!.GetAll();
            List<WebPage> pages = new List<WebPage>();
            foreach (var record in records) {
                if (record.LastFinishedExecution is not null) {
                    pages.AddRange(record.LastFinishedExecution.pages);
                }
            }
            return pages;
        }
    }
}