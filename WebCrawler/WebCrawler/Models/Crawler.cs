using System;
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
                try { 
                    pageStream = await client.GetStreamAsync(webPage.Url);
                } catch (Exception e) {
                    //log e.Message
                    return new();
                }
            }
            webPage.CrawlTime = DateTime.Now;
            //define constant patterns and regular expressions
            string line;
                       
            StreamReader reader = new StreamReader(pageStream);
            
            //parse each line
            while ((line = reader.ReadLine()!) is not null) {
                FindUrl(line, regex);
            }
            return foundWebPages;
        }

        private string? FindRefInLine(string line) {
            const string regexPattern = "(<a +href=\".*\" +>)|(<a [^<^>]* href=\".*\" [^<]*>)";
            Regex linkRegularExpression = new Regex(regexPattern, RegexOptions.Compiled);

            Match match = linkRegularExpression.Match(line);
            if (match.Success) {
                return match.Value;
            }
            return null;
        }

        private string? FindHrefInRef(string reference) {
            const string hrefPattern = "href=\".{3,\"";
            Regex hrefRegularExpression = new Regex(hrefPattern, RegexOptions.Compiled);

            Match hrefMatch = hrefRegularExpression.Match(reference);

            if (hrefMatch.Success) {
                return hrefMatch.Value;
            }
            return null;
        }

        private string? FindUrl(string line,string regex) {
            string? reference = FindRefInLine(line);

            string? href;
            if(reference is not null) {
                href = FindHrefInRef(reference);
            }

            int quotationMarksIndex = hrefMatch.Value.IndexOf(_quotationMarksString);
            var url = hrefMatch.Value.Substring(quotationMarksIndex + 1, hrefMatch.Length - quotationMarksIndex - 1);

            Regex urlRegularExpression = new Regex(regex);

            Match urlMatch = urlRegularExpression.Match(url);
            if (urlMatch.Success) {
                return url;
            }
            return null;

            foundWebPages.Add(new(url));
            webPage.OutgoingUrls.Add(url);
        }
    }
}

