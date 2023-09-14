namespace WebCrawler.Models {
    public class WebsiteRecordRepository {
        //here are stored all website records in the app
        private IList<WebsiteRecord> _records { get; set; }
        
        public WebsiteRecordRepository(IList<WebsiteRecord> records) {
            _records = records;
        }
        public IList<WebsiteRecord> GetAll() {
            return _records;
        }

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
        public void StartNewExecution(WebsiteRecord record) {
            record.StartNewExecution();
        }

        public List<Execution> GetAllExecutions(int recordId) {
            var record = Find(recordId);
            if (record is null) {
                 return new();
            }
            List<Execution> list = record.GetAllExecutions();
            return list;
        }

        public List<WebsiteRecord> GetAllRecords() {
            return (List<WebsiteRecord>) _records;
        }
    }
}
