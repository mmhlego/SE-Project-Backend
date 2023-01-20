namespace MyOnlineShop.Models.apimodel
{

	public class statsReqModel
	{
		public Guid productId { get; set; }
		public DateTime datefrom { get; set; }
		public DateTime dateto { get; set; }
	}

	public class statModel
	{
		public Guid id { get; set; }
		public Guid productId { get; set; }
		public Guid sellerId { get; set; }
		public DateTime date { get; set; }
		public int amount { get; set; }
		public double price { get; set; }

	}

}