using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCrawler.Models;

namespace WebCrawler.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private static WebsiteRecordRepository repo = new WebsiteRecordRepository();
        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            this.ViewBag.WRList = repo.GetAll();
            return View();
        }

        public IActionResult AboutProject() {
            return View();
        }

        [HttpPost]
        public IActionResult AddRecord(WebsiteRecord record) {
            record.ParseTags();
            repo.Add(record);
            return Redirect("Index"); //No
        }

        //gets brief data for all website records
        [HttpGet]
        public JsonResult GetMetaData() {
            return Json("{records:[{id:1, data:{lastExTime: 12, lastExStat: ok, runExCount: 3}}]}"); 
        }

        //gets all the data for the given website record
        [HttpGet]
        public JsonResult GetFullData() {
            return Json("{add: data}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}