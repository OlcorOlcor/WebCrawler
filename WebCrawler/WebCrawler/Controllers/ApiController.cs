using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCrawler.Models;
using WebCrawler.Controllers;
using System.Runtime.CompilerServices;
using WebCrawler.Models.Serializers;

namespace WebCrawler.Controllers
{
    public class ApiController : Controller {
        private readonly ILogger<ApiController> _logger;
        protected WebsiteRecordRepository? repo;
        public ApiController(ILogger<ApiController> logger, WebsiteRecordRepository repo) {
            this._logger = logger;
            this.repo = repo;
        }
    
		//gets all the data for the given website record
		[HttpGet]
        public JsonResult GetFullData(int recordId) {
            var record = repo!.Find(recordId);
            if (record is null) {
                return Json("{}");
            }

			JsonResult result = Json(record.ToStringJson());
            return result;
        }

        [HttpGet]
        public void StartNewExecution(int recordId) {
            var record = repo!.Find(recordId);
            if (record is not null) { 
			    repo.StartNewExecution(record);
			}
		}

        [HttpDelete]
        public void DeleteWebSiteRecord(int recordId) {
            repo!.Delete(recordId);
        }

        [HttpGet]
        public JsonResult GetGraphByIds(List<int> listId) {
            GraphDataSerializer serializer = new GraphDataSerializer();
            return Json(serializer.SerializeById(listId, repo!));
        }

        [HttpGet]
        public JsonResult GetLatestExecutions() {
            var records = repo!.GetAll();
            ExecutionSerializer serializer = new ExecutionSerializer();
            string json = serializer.SerializeLatestExecutions(records);
            return Json(json);
        }

        [HttpGet]
        public JsonResult GetWebsiteRecords() {
            var records = repo!.GetAll();
            WebsiteRecordSerializer serializer = new WebsiteRecordSerializer();
            string json = serializer.SerializeWebsiteRecords(records);
            return Json(json);
        }

        [HttpGet]
        public JsonResult GetExecutions() { 
            ExecutionSerializer es = new ExecutionSerializer();
            string json = es.SerializeAllExecutions(repo!);
            return Json(json);
        }
    }
}
