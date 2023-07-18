namespace WebCrawler.Models {
    public class WebsiteRecord {
        private static int LastID { get; set; }
        public WebsiteRecord() {
            LastID++;
            Id = LastID;
        }
        public int Id { get; init; }
        public string Url { get; set; } = "";
        public string Regex { get; set; } = "";
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public string Label { get; set; } = "";
        public string Tags { get; set; } = "";
        public string[] TagsArray { get; set; }

        //last finished execution is saved here, when next one is finished this will be rewrited by it
        public Execution LastFinishedExecution { get; set; }

        //list of all running executions of this WebsiteRecord
        public List<Execution> RunningExecutions { get; set; }

        public void ParseTags() {
            if (Tags != "" && Tags != null) {
                TagsArray = Tags.Split(',');
            }
            else {
                TagsArray = new string[0];
            }
        }
    }
}
