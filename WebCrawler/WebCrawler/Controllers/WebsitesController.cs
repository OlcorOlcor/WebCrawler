using WebCrawler.Models;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebCrawler.Controllers {

    [Route("~/graphql")]
    public class WebsitesController : GraphController {
        private readonly ILogger<ApiController> _logger;
        protected WebsiteRecordRepository? repo;
        public WebsitesController(ILogger<ApiController> logger, WebsiteRecordRepository repo) {
            this._logger = logger;
            this.repo = repo;
        }

        [Query("Websites")]
        public WebsiteRecord[] Websites() {
            return repo!.GetAll().ToArray();
        }
        [Query("Nodes")]
        public WebPage[] Nodes() {
            var records = repo!.GetAll();
            List<WebPage> pages = new List<WebPage>();
            foreach (var record in records) {
                if (record.LastFinishedExecution is not null) {
                    pages.AddRange(record.LastFinishedExecution.pages);
                }
            }
            return pages.ToArray();
        }
    }
}