using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCrawler.Models;

namespace WebCrawler.Controllers {
    public class HomeController : CrawlerController {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
            _logger.LogInformation("CTOR");
        }
        [HttpGet]
        public IActionResult Index() {
            this.ViewBag.WRList = repo.GetAll();
			_logger.LogInformation("INDEX");
			return View();
        }
        [HttpPost]
		public ContentResult Index(WebsiteRecord record) {
			record.ParseTags();
			repo.Add(record);
            repo.StartNewExecution(record);
            //VALIDATION
			this.ViewBag.WRList = repo.GetAll();
            //TODO fill missing info
            return Content($"<tr><td>{record.Url}</td><td>{record.Regex}</td><td>{record.Days.ToString()}d {record.Hours.ToString()}h {record.Minutes.ToString()}m</td><td>{record.Label}</td><td>TODO</td><td>TODO</td><td>Tags</td><td>TODO</td></tr>");
		}

		public IActionResult AboutProject() {
            return View();
        }

        //[HttpPost]
        //public void AddRecord(WebsiteRecord record) {
        //    this._logger.LogInformation("Slithin' my throat from the other side");
        //    record.ParseTags();
        //    repo.Add(record);
        //}



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}