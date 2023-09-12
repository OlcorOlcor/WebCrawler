using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace WebCrawler.Models.Serializers {
    public class ExecutionSerializer : ISerializer<Execution> {
        private StringBuilder? sb;

        public string Serialize(Execution execution) {
            //TODO: Serialize Execution for Execution table (maybe could merge/reuse SerializeExecutionForWebsiteRecordTable method
            throw new NotImplementedException();
        }
        private void SerializeExecutionForWebsiteRecordTable(Execution execution, int recordId) {
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
                    SerializeExecutionForWebsiteRecordTable(record.LastFinishedExecution, record.Id);
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }
    }
}
