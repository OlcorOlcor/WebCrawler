using Microsoft.AspNetCore.Mvc;
using WebCrawler.Models;

namespace WebCrawler.Controllers {
    public class CrawlerControler : Controller {
        protected static WebsiteRecordRepository repo = new WebsiteRecordRepository();
    }
}
