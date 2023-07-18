namespace WebCrawler.Models {
    public class WebsiteRecordRepository {
        private List<WebsiteRecord> _records { get; set; } = new();
        public List<WebsiteRecord> GetAll() {
            return _records;
        }

        //TODO add queue for executions

        public WebsiteRecord? Find(int id) {
            foreach (var record in _records) {
                if (record.Id == id) {
                    return record;
                }
            }
            return null;
        }

        public void Add(WebsiteRecord record) {
            _records.Add(record);
        }

        public void Delete(WebsiteRecord record) {
            _records.Remove(record);
        }

        public void Delete(int id) {
            var record = Find(id);
            if (record != null) {
                Delete(record);
            }
        }

        public void Update(WebsiteRecord record) {
            var index = FindIndex(record.Id);
            if (index == -1) {
                return;
            }
            _records[index] = record;
        }

        private int FindIndex(int id) {
            for (int i = 0; i < _records.Count; i++) {
                if (_records[i].Id == id)
                    return i;
            }
            return -1;
        }
    }
}
