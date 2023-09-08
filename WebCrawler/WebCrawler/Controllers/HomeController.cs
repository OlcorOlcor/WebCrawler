using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCrawler.Models;

namespace WebCrawler.Controllers {
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;
        protected WebsiteRecordRepository? repo;
        public HomeController(ILogger<HomeController> logger, WebsiteRecordRepository repository) {
            _logger = logger;
            repo = repository;
        }
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

        //this is only for testing crawler and can be removed later
        public IActionResult TestProject() {
            var record = new WebsiteRecord() {
                Url = "https://cs.wikipedia.org/wiki/Stopa%C5%99%C5%AFv_pr%C5%AFvodce_po_Galaxii",
                //Url = "http://www.ms.mff.cuni.cz/~zikmundr/",
                Regex = ".*cs.*.org.*"
            };

            var record2 = new WebsiteRecord() {
                Url = "https://cs.wikipedia.org/wiki/National_Basketball_Association",
                Regex = ".*bask.*"
            };

            Console.WriteLine("Test started.");
            Console.WriteLine(record2.Id);

            repo!.Add(record);
            repo!.Add(record2);
            repo.StartNewExecution(record);
            Thread.Sleep(1000);
            repo.StartNewExecution(record2);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddRecord(WebsiteRecord record) {
            record.ParseTags();
            repo.Add(record);
            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}