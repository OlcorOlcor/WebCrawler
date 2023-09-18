using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace WebCrawler.Models {

    public class Crawler {
        //strings to be found in page for identifiing references
        private const string _refString = "<a href=\"" ;
        private const string _quotationMarksString = "\"";

        private static readonly HashSet<string> _resourceFileExtentions = new HashSet<string> {
            "jpeg", "jpg",
            "png",
            "svg",
            "json",
            "pdf",

            // TODO ADD MORE!!!!
        };

        public async Task<Website> CrawlSite(string url, string regex) {
            Console.WriteLine("Crawling " + url);

            //get data from server
            Stream pageStream;
            using (var client = new HttpClient()) {
                try {
                    pageStream = await client.GetStreamAsync(url);
                } catch (Exception e) {
                    await Console.Out.WriteLineAsync(e.Message);
                    return new Website(url, "", new WebLinks(), DateTime.Now, false);
                }
            }

            //define constant patterns and regular expressions
            StreamReader reader = new StreamReader(pageStream);
            Regex linkRegex = new Regex(regex);

            string title = GetPageTitle(reader);

            var matchingLinks = new List<string>();
            var notMatchingLinks = new List<string>();
            string line;
            //parse each line
            while ((line = reader.ReadLine()!) is not null) {
                var linksFound = FindLinksInLine(line);
                foreach (var foundLink in linksFound) {
                    string link = foundLink;
                    if (IsRelativeUrl(foundLink)) {
                        link = url + foundLink;
                    }

                    if (IsResource(link)) {
                        Console.WriteLine($"Ommited resource link: {link}");
                        continue;
                    }
                    //link = Uri.EscapeDataString(link);
                    if (!Uri.IsWellFormedUriString(link, UriKind.Absolute)) {
                        Console.WriteLine($"Ommited corrupted link: {link}");
                        continue;
                    }

                    Match match = linkRegex.Match(link);
                    if (match.Success) {
                        matchingLinks.Add(link);
                    }
                    else {
                        notMatchingLinks.Add(link);
                    }
                }
            }

            await Console.Out.WriteLineAsync("Matching outgoing URLs count: " + matchingLinks.ToArray().Length.ToString());

            var outgoingLinks = new WebLinks() {
                UrlsMatchingRegex = matchingLinks.ToArray(), 
                UrlsNotMatchingRegex = notMatchingLinks.ToArray()
            };
            return new Website(url, title, outgoingLinks, DateTime.Now, true);
        }

        private bool IsRelativeUrl(string url) {
            return url[0].ToString() == "/" || url[0].ToString() == "?" || url[0].ToString() == "#";
        }

        private bool IsResource(string url) {
            var splitUrl = url.Split('.');
            var potentialExtention = splitUrl[splitUrl.Length - 1];

            return _resourceFileExtentions.Contains(potentialExtention);
        }

        //returns a reference html component from given line or null if none present
        private List<string> FindRefInLine(string line) {
            const string regexPattern = "(<a +href=\"[^<>]*\"[^<]*>)|(<a [^<>]* href=\"[^<>]*\"[^<]+>)";
            Regex linkRegularExpression = new Regex(regexPattern, RegexOptions.Compiled);

            List<string> references = new();

            var matches = linkRegularExpression.Matches(line);
            foreach (Match match in matches) {
                if (match.Success) {
                    references.Add(match.Value);
                }
            }
            return references;
        }

        //returns href part from given a reference html component or null if none present
        private string? FindHrefInRef(string reference) {
            const string hrefPattern = "href=\"[^\"]{3,}\"";
            Regex hrefRegularExpression = new Regex(hrefPattern, RegexOptions.Compiled);

            Match hrefMatch = hrefRegularExpression.Match(reference);

            if (hrefMatch.Success) {
                return hrefMatch.Value;
            }
            return null;
        }

        private List<string> FindLinksInLine(string line) {
            List<string> links = new();
            List<string> references = FindRefInLine(line);

            foreach (var reference in references) { 
                string? href;
                href = FindHrefInRef(reference);

                if (href is not null) {
                    string? link;
                    link = FindLinkInHref(href);

                    if (link is not null) {
                        links.Add(link);
                    }
                }
            }
            return links;
        }

        //returns url from given html href section
        private string? FindLinkInHref(string href) {
            int quotationMarksIndex = href.IndexOf(_quotationMarksString);
            var url = href.Substring(quotationMarksIndex + 1, href.Length - quotationMarksIndex - 2);
            return url;
        }

        //returns contents of title element from given string in stream reader
        private string GetPageTitle(StreamReader reader) {
            string line;
            while ((line = reader.ReadLine()!) is not null) {
                string? titlePart = FindTitleInLine(line);
                if(titlePart is not null) {
                    string? title = FindTitleInPart(titlePart);
                    if(title is not null) {
                        return title;
                    }
                }
            }
            //cannot happen but c# needs this
            return string.Empty;
        }

        //returns whole title element from line or null if there is no title element in this line
        private string? FindTitleInLine(string line) {
            const string titleRegexPattern = "<title>.*</title>";
            Regex linkRegularExpression = new Regex(titleRegexPattern, RegexOptions.Compiled);

            Match match = linkRegularExpression.Match(line);
            if(match.Success) {
                return match.Value;
            }
            return null;
        }

        //returns title text from given title element or null if the string is not in required format
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

