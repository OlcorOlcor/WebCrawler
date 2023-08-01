﻿namespace WebCrawler.Models {
    public class Crawler {
        private static string RefString = "<a href=\"";
        private static string QuatationMarksString = "\"";

        public List<WebPage> CrawlSite(WebPage webPage, string regex) {
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
                        Console.WriteLine(currentUrl);
                    }
                }
            }
        }
    }
}
