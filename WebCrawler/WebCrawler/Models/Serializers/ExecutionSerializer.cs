using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler.Models.Serializers {
    public class ExecutionSerializer : ISerializer<Execution> {
        private StringBuilder? sb;

        public string SerializeAllExecutions(WebsiteRecordRepository repository) {
            sb = new StringBuilder();
            sb.Append("{\"Executions\":[");
            bool isFirst = true;
            foreach (var record in repository.GetAllRecords()) {
                foreach (Execution execution in record.GetAllExecutions()) {
                    if (isFirst) {
                        sb.Append(Serialize(execution));
                        isFirst = false;
                    }
                    else {
                        sb.Append("," + Serialize(execution));
                    }
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }

        public string Serialize(Execution execution) {
            //TODO: Serialize Execution for Execution table (maybe could merge/reuse SerializeExecutionForWebsiteRecordTable method
            throw new NotImplementedException();
        }
        private void SerializeExecutionForWebsiteRecordTable(Execution execution, int recordId, string recordLabel) {
            if (sb is null) {
                return;
            }
            sb.Append("{");
            sb.Append($"\"RecordId\": {recordId},"); //probably should not be displayed for user
            sb.Append($"\"RecordLabel\": {recordLabel},");
            sb.Append($"\"Time\": \"{execution.ExecutionTime}\",");
            sb.Append($"\"Status\": {execution.Status.ToString()},");
            sb.Append($"\"NumberOfSitesCrawled\": {execution.pages.Count}");
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
                    SerializeExecutionForWebsiteRecordTable(record.LastFinishedExecution, record.Id, record.Label!);
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }
    }
}
