using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCrawler.Models;

namespace WebCrawler.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            this.ViewBag.Name = "Honza";
            this.ViewData["Name"] = "Honza";
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
            if (ViewBag["WRList"] is null) {
                ViewBag["WRList"] = new List<WebsiteRecord>();
            }
            //TODO: Do this better - prolly in another class
            ((List<WebsiteRecord>)ViewBag["WRList"]).Add(record);
            return View();
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