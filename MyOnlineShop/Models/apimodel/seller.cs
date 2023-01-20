namespace MyOnlineShop.Models.apimodel
{
	public class SellerSchema
	{
		public Guid id { get; set; }
		public string name { get; set; }
		public string image { get; set; }
		public string address { get; set; }
		public string information { get; set; }
		public int likes { get; set; }
		public int dislikes { get; set; }
		public bool restricted { get; set; }
	}


	public class SellerpagePutMethodRequest
	{
		public string address { get; set; }
		public string information { get; set; }
	}
}

