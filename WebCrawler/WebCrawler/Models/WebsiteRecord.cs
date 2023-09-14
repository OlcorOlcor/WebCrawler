using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Timers;

namespace WebCrawler.Models {
    public class WebsiteRecord {
        private static int _nextID { get; set; }
        public WebsiteRecord() {
            Id = _nextID;
            _nextID++;
        }
        public int Id { get; init; }

        [Url(ErrorMessage = "Not a valid URL.")]
        [Required(ErrorMessage = "URL is required.")]
        public string? Url { get; set; }

        [Required(ErrorMessage = "Regex is required.")]
        public string? Regex { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int? Days { get; set; }

        [Required]
        [Range(0,23)]
        public int? Hours { get; set; }

        [Required]
        [Range(0,59)]
        public int? Minutes { get; set; }

        [Required]
        public string? Label { get; set; }

        public string? Tags { get; set; }
        public string[] TagsArray { get; set; } = new string[0];
        public bool Active { get; set; } = true;

        //last finished execution is saved here, when next one is finished this will be rewrited by it
        public Execution? LastFinishedExecution { get; set; } = null;

        //list of all running executions of this WebsiteRecord
        public List<Execution> RunningExecutions { get; set; } = new List<Execution>();

        public void ParseTags() {
            if (Tags != "" && Tags != null) {
                TagsArray = Tags.Split(',');
            }
            else {
                TagsArray = new string[0];
            }
        }

        public void StartNewExecution() {
            if (this.Url is null || this.Regex is null) {
                throw new InvalidDataException();
            } 

            Execution execution = new Execution(this.Url, this.Regex);
            execution.updateRepositoryCallback = ExecutionFinished;
            this.RunningExecutions.Add(execution);
            ThreadPool.QueueUserWorkItem(execution.Execute);
        }

        public List<Execution> GetAllExecutions() {
            List<Execution> list = new List<Execution>();
            if (LastFinishedExecution is not null) {
                list.Add(LastFinishedExecution);
            }
            list.AddRange(this.RunningExecutions);
            return list;
        }

        private void ExecutionFinished(Execution execution) {
            //the exectuion in the parameter is from another thread so the following line might not word :c
            var executionIndex = RunningExecutions.IndexOf(execution);
            RunningExecutions.Remove(RunningExecutions[executionIndex]);
            this.LastFinishedExecution = execution;

            if (RunningExecutions.Count == 0 && Active == true) {
                StartNewExecution();
                return;
            }
            else if (Active == true) {
                double interval = GetPeriodicityInMiliseconds();
                System.Timers.Timer timer = new System.Timers.Timer(interval);
                
                timer.Elapsed += CheckAndStartNewExecution;

                // Do not repeat
                timer.AutoReset = false;

                // Start the timer
                timer.Enabled = true;
            }
        }

        private void CheckAndStartNewExecution(object? source, System.Timers.ElapsedEventArgs e) {
            if (!Active) {
                return;
            }
            Console.WriteLine("Elapsed");
            StartNewExecution();
        }

        private double GetPeriodicityInMiliseconds() {
            if (this.Days is null || this.Hours is null || this.Minutes is null) {
                throw new InvalidDataException();
            }

            double daysInMiliseconds = (double)(this.Days * 24 * 60 * 60 * 1000);
            double hoursInMiliseconds = (double)(this.Hours * 60 * 60 * 1000);
            double minutesInMiliseconds = (double)(this.Minutes * 60 * 1000);

            var milis = daysInMiliseconds + hoursInMiliseconds + minutesInMiliseconds;
            if (milis == 0) {
                return 1d;
            }
            return milis;
        }
    }
}
