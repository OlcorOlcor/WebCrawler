using System.Text;

namespace WebCrawler.Models.Serializers {
    public class WebsiteRecordSerializer : ISerializer<WebsiteRecord> {
        public string Serialize(WebsiteRecord record) {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("{");
            sb.Append($"\"Id\": {record.Id}");
            sb.Append($",\"Url\": \"{record.Url}\"");
            sb.Append($",\"Regex\": \"{record.Regex!.Replace("\"", "\\\"").Replace("\\", "\\\\")}\"");
            sb.Append($",\"Days\": {record.Days}");
            sb.Append($",\"Hours\": {record.Hours}");
            sb.Append($",\"Minutes\": {record.Minutes}");
            sb.Append($",\"Label\": \"{record.Label!}\"");

            sb.Append(",\"Tags\": [");
            bool first = true;
            foreach (var tag in record.TagsArray) {
                if (first) {
                    sb.Append($"\"{tag}\"");
                    first = false;
                } else {
                    sb.Append($",\"{tag}\"");
                }
            }
            sb.Append("]");

            if (record.LastFinishedExecution is not null) {
                sb.Append($",\"LastExecutionTime\": \"{record.LastFinishedExecution.ExecutionTime}\"");
                sb.Append($",\"LastExecutionStatus\": \"{record.LastFinishedExecution.Status}\"");
            } else {
                sb.Append($",\"LastExecutionTime\": \"No record has finished\"");
                sb.Append($",\"LastExecutionStatus\": \"running\"");
            }
            sb.Append("}");
            return sb.ToString();
        }

        public string SerializeWebsiteRecords(IList<WebsiteRecord> records) {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            sb.Append("\"WebsiteRecords\": [");
            bool first = true;
            foreach (var record in records) {
                if (first) {
                    sb.Append(Serialize(record));
                    first = false;
                } else {
                    sb.Append("," + Serialize(record));
                }
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
