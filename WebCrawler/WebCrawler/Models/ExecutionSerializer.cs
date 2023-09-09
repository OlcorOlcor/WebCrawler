using System.Text;

namespace WebCrawler.Models {
    public class ExecutionSerializer {
        private StringBuilder sb = new StringBuilder();
        private void SerializeExecution(Execution execution, int recordId) {
            sb.Append("{");
            sb.Append($"\"RecordId\": {recordId},");
            sb.Append($"\"Time\": {execution.ExecutionTime},");
            sb.Append($"\"Status\": FINISHED"); //TODO: Change to actual status once implemented
            sb.Append("}");
        }
        public string SerializeLatestExecutions(List<WebsiteRecord> records) {
            sb.Append("{ \"Executions\": [");
            foreach (WebsiteRecord record in records) {
                SerializeExecution(record.LastFinishedExecution, record.Id);
            }
            sb.Append("]}");
            return sb.ToString();
        }
    }
}
