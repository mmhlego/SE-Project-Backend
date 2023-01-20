namespace MyOnlineShop.Models.apimodel
{
	public class Pagination<T>
	{
		public int perPage { get; set; }
		public int page { get; set; }
		public int totalPages { get; set; }
		public List<T> data { get; set; }
	}
}
