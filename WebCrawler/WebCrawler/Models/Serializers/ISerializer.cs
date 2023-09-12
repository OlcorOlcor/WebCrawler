namespace WebCrawler.Models.Serializers {
    public interface ISerializer<T> {
        public string Serialize(T obj);
    }
}
