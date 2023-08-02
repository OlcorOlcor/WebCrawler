namespace WebCrawler.Models {
    public class Crawler {
        //strings to be found in page for identifiing references
        private static string RefString = "<a href=\"";
        private static string QuatationMarksString = "\"";

        //TODO add delegate to return List<WebaPage> to
        //and where crawler updates info: title and time

        public List<WebPage> CrawlSite(WebPage webPage, string regex) {
            //TODO start timer ??

            //list to be filled with new, not yet crawled webpages
            List<WebPage> foundWebPages = new();

            //get data from server
            //there should not be problem with size, because data are nomaly-sized webpages
            using var client = new HttpClient();
            Task<string> responseGet = client.GetStringAsync(webPage.Url);
            responseGet.Wait();
            string pageString = responseGet.Result;

            //search in data for reference
            while (true) {
                int indexOfFirstRef = pageString.IndexOf(RefString);

                //end loop if there is non left
                if(indexOfFirstRef == -1) {
                    break;
                }
                else {
                    //cut page up to current ref
                    pageString = pageString.Substring(indexOfFirstRef + RefString.Length);
                    //gets current url
                    int indexOfNextQuatationMarks = pageString.IndexOf(QuatationMarksString);
                    string currentUrl = pageString.Substring(0, indexOfNextQuatationMarks);
                    //check if ref leads to walid url
                    //TODO not checking properly
                    Console.WriteLine(currentUrl);
                    if (new Uri(currentUrl).Scheme == "http" || new Uri(currentUrl).Scheme == "https") {
                        //add new webpage with given url to list
                        foundWebPages.Add(new WebPage(currentUrl));
                    }
                }
            }

            //TODO end timer ??

            return foundWebPages;
        }
    }
}

