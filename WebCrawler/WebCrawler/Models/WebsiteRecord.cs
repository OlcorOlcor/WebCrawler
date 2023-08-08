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
        public List<Execution> RunningExecutions { get; set; } = new();

        public void ParseTags() {
            if (Tags != "" && Tags != null) {
                TagsArray = Tags.Split(',');
            }
            else {
                TagsArray = new string[0];
            }
        }
        public Execution StartNewExecution() {
            Execution execution = new Execution(this.Url, this.Regex);
            execution.callbackMethod = ExecutionFinished;
            this.RunningExecutions.Add(execution);
            return execution;
        }
        private void ExecutionFinished(Execution execution) {
            //the exectuion in the parameter is from another thread so the following line might not word :c
            var executionIndex = RunningExecutions.IndexOf(execution);
            RunningExecutions.Remove(RunningExecutions[executionIndex]);
            this.LastFinishedExecution = execution;
        }

        public List<Execution> GetAllExecutions() {
            List<Execution> list = new List<Execution>();
            if (LastFinishedExecution is not null) {
                list.Add(LastFinishedExecution);
            }
            list.AddRange(this.RunningExecutions);
            return list;
        }
    }
}
