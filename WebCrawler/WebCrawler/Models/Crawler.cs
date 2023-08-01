namespace WebCrawler.Models {
    public class Crawler {
        public List<WebPage> CrawlSite(WebPage webPage, string regex) {
            List<WebPage> foundWebPages = new();

            //get data from server
            //there should not be problem with size, because data are nomaly-sized webpages
            using var client = new HttpClient();
            Task<string> responseGet = client.GetStringAsync(webPage.Url);
            responseGet.Wait();


        }
    }
}
