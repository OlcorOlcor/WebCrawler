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

            webPage.Title = GetPageTitle(reader);

            //parse each line
            while ((line = reader.ReadLine()!) is not null) {
                string? url = FindUrl(line, regex);
                if(url is not null) {
                    foundWebPages.Add(new(url));
                    webPage.OutgoingUrls.Add(url);
                }
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

            if(reference is not null) {
                string? href;
                href = FindHrefInRef(reference);

                if(href is not null) {
                    string? url;
                    url = FindUrlInHref(href);

                    if (url is not null) {
                        return url;
                    }
                }
            }
            return null;
        }

        private string? FindUrlInHref(string href) {
            int quotationMarksIndex = href.IndexOf(_quotationMarksString);
            var url = href.Substring(quotationMarksIndex + 1, href.Length - quotationMarksIndex - 1);
            return url;
        }

        private string GetPageTitle(StreamReader reader) {
            string line;
            while ((line = reader.ReadLine()!) is not null) {
                string? titlePart = FindTitleInLine(line);
                if(titlePart is not null) {
                    string? title = FindTitleInLine(titlePart);
                    if(title is not null) {
                        return title;
                    }
                }
            }
            //cannot happen but c# neads this
            return string.Empty;
        }

        private string? FindTitleInLine(string line) {
            const string titleRegexPattern = "<title>.*</title>";
            Regex linkRegularExpression = new Regex(titleRegexPattern, RegexOptions.Compiled);

            Match match = linkRegularExpression.Match(line);
            if(match.Success) {
                return match.Value;
            }
            return null;
        }

        private string? FindTitleInPart(string part) {
            const int titleStartElementLength = 7;
            const int titleEndElementLength = 8;
            string? title;
            try {
                title = part.Substring(titleStartElementLength, part.Length - titleEndElementLength - titleStartElementLength);
            }
            catch (Exception e) when (e is ArgumentOutOfRangeException){
                title = null;
            }
            return title;
        }
    }
}

