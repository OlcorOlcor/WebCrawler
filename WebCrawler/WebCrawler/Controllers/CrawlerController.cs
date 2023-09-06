using Microsoft.AspNetCore.Mvc;
using WebCrawler.Models;

namespace WebCrawler.Controllers {
    public class CrawlerController : Controller {
        protected static WebsiteRecordRepository repo = new WebsiteRecordRepository();
    }
}
