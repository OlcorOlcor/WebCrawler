using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
            if (!ValidateWebRecord(record)) { 
                return Content("");
            }


            record.ParseTags();
			repo!.Add(record);
            repo.StartNewExecution(record);

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

        private bool ValidateWebRecord(WebsiteRecord record) {
            bool valid = true;
            //Validation of defined attributes
            if (!ModelState.IsValid) {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                _logger.Log(LogLevel.Information, "WebRecord validation failed:");
                foreach (var error in errors) {
                    _logger.Log(LogLevel.Error, error.ErrorMessage);
                }
                valid = false;
            }
            //Validation of periodicity
            if (record.Days + record.Hours + record.Minutes < 0) {
                _logger.Log(LogLevel.Error, "Periodicy is not set");
                valid = false;
            }
            //Validation of URI
            string? escapedInputUrl = null;
            if (record.Url is null) {
                _logger.Log(LogLevel.Error, "URL is not well formated");
                valid = false;
            }
            else {
                escapedInputUrl = Uri.EscapeDataString(record.Url);
            }
            if (Uri.IsWellFormedUriString(escapedInputUrl, UriKind.Absolute)) {
                _logger.Log(LogLevel.Error, "URL is not well formated");
                valid = false;
            }
            //Validation of Regular Expression
            try {
                Regex regex = new Regex(record.Regex!);
            } catch (ArgumentException) {
                _logger.Log(LogLevel.Error, "Regex is not well formated");
                valid = false;
            }
            return valid;
        }
    }
}