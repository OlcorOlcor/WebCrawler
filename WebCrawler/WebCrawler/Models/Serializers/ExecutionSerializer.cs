﻿using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler.Models.Serializers {
    public class ExecutionSerializer : ISerializer<Execution> {
        private StringBuilder? sb;

        public string Serialize(Execution execution) {
            throw new NotImplementedException();
        }

        public string SerializeAllExecutions(WebsiteRecordRepository repository) {
            sb = new StringBuilder();
            sb.Append("{\"Executions\":[");
            bool isFirst = true;
            foreach (var record in repository.GetAllRecords()) {
                foreach (Execution execution in record.GetAllExecutions()) {
                    if (isFirst) {
                        sb.Append(SerializeForExecutionTable(execution, record.Id, record.Label!));
                        isFirst = false;
                    }
                    else {
                        sb.Append("," + SerializeForExecutionTable(execution, record.Id, record.Label!));
                    }
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }

        public string SerializeForExecutionTable(Execution execution, int recordId, string recordLabel) {
            StringBuilder sb2 = new StringBuilder();
            sb2.Append("{");
            sb2.Append($"\"RecordId\": {recordId},"); //is not displayed to user, but important for filtering
            sb2.Append($"\"RecordLabel\": \"{recordLabel}\",");
            sb2.Append($"\"StartTime\": \"{execution.Start}\",");
            if(execution.End == null) {
                sb2.Append($"\"EndTime\": \"-\",");
            }
            else {
                sb2.Append($"\"EndTime\": \"{execution.End}\",");
            }
            sb2.Append($"\"Status\": \"{execution.Status.ToString()}\",");
            sb2.Append($"\"NumberOfSitesCrawled\": {execution.pages.Count}");
            sb2.Append("}");
            return sb2.ToString();
        }

        private void SerializeExecutionForWebsiteRecordTable(Execution execution, int recordId) {
            if (sb is null) {
                return;
            }
            sb.Append("{");
            sb.Append($"\"RecordId\": {recordId},");
            //TODO execution time does nothing right now, Execution table uses Start/End time instead
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
