using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace WebCrawler.Models.Serializers {
    public class ExecutionSerializer {
        private StringBuilder? sb;
        private void SerializeExecution(Execution execution, int recordId) {
            if (sb is null) {
                return;
            }
            sb.Append("{");
            sb.Append($"\"RecordId\": {recordId},");
            sb.Append($"\"Time\": \"{execution.ExecutionTime}\",");
            sb.Append($"\"Status\": \"{execution.Status}\"");
            sb.Append("}");
        }
        public string SerializeLatestExecutions(IList<WebsiteRecord> records) {
            sb = new StringBuilder();
            sb.Append("{ \"Executions\": [");
            bool firstRecord = true;
            foreach (WebsiteRecord record in records) {
                if (record.LastFinishedExecution is not null) {
                    if (!firstRecord) {
                        sb.Append(",");
                    }
                    SerializeExecution(record.LastFinishedExecution, record.Id);
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }
    }
}
