using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCrawler.Models;
using WebCrawler.Controllers;
using System.Runtime.CompilerServices;

namespace WebCrawler.Controllers {
  public class ApiController : Controller {
        private readonly ILogger<ApiController> _logger;
        protected WebsiteRecordRepository? repo;
        public ApiController(ILogger<ApiController> logger, WebsiteRecordRepository repo) {
            this._logger = logger;
            this.repo = repo;
        }
        //gets brief data for all website records
        [HttpGet]
        public JsonResult GetMetaData() {
            return Json("{records:[{id:1, data:{lastExTime: 12, lastExStat: ok, runExCount: 3}}]}"); 
        }

		private static int i = 0;
        private static string nodes = "{\"nodes\":[{\"id\":\"Napoleon\",\"group\":1},{\"id\":\"Myriel\",\"group\":1}";
        private static string links = "],\"links\":[{\"source\":\"Napoleon\",\"target\":\"Myriel\",\"value\":1}";
        private string end = "]}";

		//gets all the data for the given website record
		[HttpGet]
        public JsonResult GetFullData(int recordId) {
            var record = repo.Find(recordId);
            if (record is null) {
                return Json("{}");
            }

			JsonResult result = Json(record.ToStringJson());
            return result;

        }
        [HttpPost]
        public void StartNewExecution(int recordId) {
            var record = repo.Find(recordId);
            if (record is not null) { 
			    repo.StartNewExecution(record);
			}
		}
    }
}