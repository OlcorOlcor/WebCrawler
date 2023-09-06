using System.Numerics;

namespace WebCrawler.Models {
    public class Planner {
        public void PlanNewExecution(Execution execution) {
            ThreadPool.QueueUserWorkItem(execution.Execute);
        }
    }
}
