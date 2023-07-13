using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCrawler.Models;

namespace WebCrawler.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private static List<WebsiteRecord> _records = new();
        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            this.ViewBag.Name = "Honza";
            this.ViewData["Name"] = "Honza";
            this.ViewBag.WRList = _records;
            return View();
        }

        public IActionResult AddWebsiteRecord() {
            return View();
        }

        public IActionResult AboutProject() {
            return View();
        }
        [HttpPost]
        public IActionResult AddRecord(WebsiteRecord record) {
            //TODO: Do this better - prolly in another class
            _records.Add(record);
            return Redirect("Index");
        }
        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}