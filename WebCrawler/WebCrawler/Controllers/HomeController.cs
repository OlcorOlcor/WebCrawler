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
        public IActionResult Index() {
            this.ViewBag.WRList = repo.GetAll();
			_logger.LogInformation("INDEX");
			return View();
        }
        [HttpPost]
		public ContentResult Index(WebsiteRecord record) {
			record.ParseTags();
			repo!.Add(record);
            repo.StartNewExecution(record);
            //VALIDATION
			this.ViewBag.WRList = repo.GetAll();
            //TODO fill missing info
            return Content(
                $"" +
                $"<tr>" +
                $"  <td>{record.Url}</td>" +
                $"<td>{record.Regex}</td>" +
                $"<td>{record.Days.ToString()}d {record.Hours.ToString()}h {record.Minutes.ToString()}m</td>" +
                $"<td>{record.Label}</td>" +
                $"<td id=\"ExecutionTime{record.Id}\">YET TO FINISH</td>" +
                $"<td id=\"ExecutionStatus{record.Id}\">RUNNING</td>" +
                $"<td>Tags</td>" +
                $"<td><button class=\"btn btn-primary\" onClick=\"startExecution({record.Id})\">Start New Execution</button></td>" +
                $"</tr>");
		}

		public IActionResult AboutProject() {
            return View();
        }
        
        [HttpPost]
        public IActionResult AddRecord(WebsiteRecord record) {
            record.ParseTags();
            repo!.Add(record);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}