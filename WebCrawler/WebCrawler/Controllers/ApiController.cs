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

        //gets brief data for all website records
        [HttpGet]
        public JsonResult GetMetaData() {
            return Json("{records:[{id:1, data:{lastExTime: 12, lastExStat: ok, runExCount: 3}}]}"); 
        }
    
		//gets all the data for the given website record
		[HttpGet]
        public JsonResult GetFullData(int recordId) {
            //Console.WriteLine($"API call: Api/GetFullData/?recordId={recordId}");
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

        [HttpGet]
        public JsonResult GetLatestExecutions() {
            var records = repo!.GetAll();
            ExecutionSerializer serializer = new ExecutionSerializer();
            string json = serializer.SerializeLatestExecutions(records);
            return Json(json);
        }
    }
}
