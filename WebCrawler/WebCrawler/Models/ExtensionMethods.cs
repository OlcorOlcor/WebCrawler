using Microsoft.AspNetCore.Mvc;

namespace WebCrawler.Models {
	public static class WebsiteRecordExtensionMethods {
		public static string ToStringJson(this WebsiteRecord record) {
			GraphDataSerializer serializer = new GraphDataSerializer();
			return serializer.SerializeRecord(record);
		}
	}
}
