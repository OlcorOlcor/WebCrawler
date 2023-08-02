using System.Text.RegularExpressions;

namespace WebCrawler.Models {
    public class Crawler {
        //strings to be found in page for identifiing references
        private const string _refString = "<a href=\"" ;
        private const string _quotationMarksString = "\"";

        //TODO add delegate to return List<WebaPage> to
        //and where crawler updates info: title and time

        public async Task<List<WebPage>> CrawlSite(WebPage webPage, string regex) {
            //TODO start timer ??

            //list to be filled with found webpages
            List<WebPage> foundWebPages = new();

            //get data from server
            //there should not be problem with size, because data are nomaly-sized webpages
            Stream pageStream;
            using (var client = new HttpClient()) {
                pageStream = await client.GetStreamAsync(webPage.Url);
            }
            string line;
            const string regexPattern = "(<a +href=\".*\" +>)|(<a [^<^>]* href=\".*\" [^<]*>)";
            const string hrefPattern = "href=\".{3,\"";
            StreamReader reader = new StreamReader(pageStream);
            Regex linkRegularExpression = new Regex(regexPattern, RegexOptions.Compiled);
            Regex hrefRegularExpression = new Regex(hrefPattern, RegexOptions.Compiled);
            Regex urlRegularExpression = new Regex(regex);
            while ((line = reader.ReadLine()!) is not null) {
                Match match = linkRegularExpression.Match(line);
                if (match.Success) {
                    Match hrefMatch = hrefRegularExpression.Match(match.Value);
                    if (hrefMatch.Success) {
                        int quotationMarksIndex = hrefMatch.Value.IndexOf(_quotationMarksString);
                        var url = hrefMatch.Value.Substring(quotationMarksIndex + 1, hrefMatch.Length - quotationMarksIndex - 1);
                        Match urlMatch = urlRegularExpression.Match(url);
                        if (urlMatch.Success) {
                            foundWebPages.Add(new(url));
                        }
                    }
                }
            }

            /*
            //search in data for reference
            while (true) {
                int indexOfFirstRef = pageString.IndexOf(RefString);

                //end loop if there is non left
                if(indexOfFirstRef == -1) {
                    break;
                }

                //cut page up to current ref
                pageString = pageString.Substring(indexOfFirstRef + RefString.Length);
                //gets current url
                int indexOfNextQuatationMarks = pageString.IndexOf(QuatationMarksString);
                string currentUrl = pageString.Substring(0, indexOfNextQuatationMarks);
                //check if ref leads to valid url
                //TODO not checking properly
                Console.WriteLine(currentUrl);
                if (new Uri(currentUrl).Scheme == "http" || new Uri(currentUrl).Scheme == "https") {
                    //add new webpage with given url to list
                    foundWebPages.Add(new WebPage(currentUrl));
                }
            }
            */
            //TODO end timer ??
            return foundWebPages;
        }
    }
}

