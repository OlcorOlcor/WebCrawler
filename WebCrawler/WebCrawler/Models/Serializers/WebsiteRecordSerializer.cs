﻿using System.Text;

namespace WebCrawler.Models.Serializers {
    public class WebsiteRecordSerializer : ISerializer<WebsiteRecord> {
        public string Serialize(WebsiteRecord record) {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("{");
            sb.Append($"\"Id\": {record.Id}");
            sb.Append($",\"Url\": \"{record.Url}\"");
            sb.Append($",\"Regex\": \"{record.Regex}\"");
            sb.Append($",\"Days\": {record.Days}");
            sb.Append($",\"Hours\": {record.Hours}");
            sb.Append($",\"Minutes\": {record.Minutes}");
            sb.Append($",\"Label\": {record.Label}");

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
            sb.Append("}");
            return sb.ToString();
        }

        public string SerializeWebsiteRecords(List<WebsiteRecord> records) {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            sb.Append("\"WebsiteRecords\": [");
            bool first = true;
            foreach (var record in records) {
                if (first) {
                    sb.Append(Serialize(record));
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
