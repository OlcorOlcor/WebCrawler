using System.Text.RegularExpressions;

namespace WebCrawler.Models {
    public class Crawler {
        //strings to be found in page for identifiing references
        private const string _refString = "<a href=\"" ;
        private const string _quotationMarksString = "\"";

        //TODO add delegate to return List<WebaPage> to
        //and where crawler updates info: title and time

        public async Task<List<WebPage>> CrawlSite(WebPage webPage, string regex) {

            //list to be filled with found webpages
            List<WebPage> foundWebPages = new();

            //get data from server
            Stream pageStream;
            using (var client = new HttpClient()) {
                pageStream = await client.GetStreamAsync(webPage.Url);
            }
            //define constant patterns and regular expressions
            string line;
            const string regexPattern = "(<a +href=\".*\" +>)|(<a [^<^>]* href=\".*\" [^<]*>)";
            const string hrefPattern = "href=\".{3,\"";
            StreamReader reader = new StreamReader(pageStream);
            Regex linkRegularExpression = new Regex(regexPattern, RegexOptions.Compiled);
            Regex hrefRegularExpression = new Regex(hrefPattern, RegexOptions.Compiled);
            Regex urlRegularExpression = new Regex(regex);
            //parse each line
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
                            webPage.OutgoingUrls.Add(url);
                        }
                    }
                }
            }
            return foundWebPages;
        }
    }
}

