using System.Numerics;

namespace WebCrawler.Models {
    public class Manager {
        public void PlanNewExecution(Execution execution) {
            ThreadPool.QueueUserWorkItem(execution.Execute);
        }
    }
}
