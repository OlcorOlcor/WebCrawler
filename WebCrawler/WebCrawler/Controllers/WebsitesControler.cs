using WebCrawler.Models;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;

namespace WebCrawler.Controllers {
    public class WebsitesController : GraphController {
        private readonly ILogger<ApiController> _logger;
        protected WebsiteRecordRepository? repo;
        public WebsitesController(ILogger<ApiController> logger, WebsiteRecordRepository repo) {
            this._logger = logger;
            this.repo = repo;
        }

        [QueryRoot]
        public WebsiteRecord[] Websites() {
            return repo!.GetAll().ToArray();
        }
    }
}