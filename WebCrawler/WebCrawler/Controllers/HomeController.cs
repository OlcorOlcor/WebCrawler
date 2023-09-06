﻿using Microsoft.AspNetCore.Identity;
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
        public IActionResult Test() {
            repo._records.Add(new());
            Console.WriteLine(repo._records.Count());
            return RedirectToAction("AboutProject");
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



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}