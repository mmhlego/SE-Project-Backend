using System;
namespace MyOnlineShop.Models.apimodel
{
	public class ProductPageGetRequestModel
	{
		public ProductPageGetRequestModel()
		{
			productsPerPage = 50;
			page = 1;
			priceTo = 1000000000000;
			priceFrom = 0;
			available = true;

		}
		public int productsPerPage { get; set; }
		public int page { get; set; }
		public double priceFrom { get; set; }
		public double priceTo { get; set; }
		public Boolean available { get; set; }
		public string catagory { get; set; }

	}
	public class ProductPagePostRequestModel
	{
		public string name { get; set; }
		public string category { get; set; }
		public string image { get; set; }
		public string description { get; set; }
	}
	public class productModel
	{
		public Guid id { get; set; }
		public string name { get; set; }
		public string category { get; set; }
		public string image { get; set; }
		public string description { get; set; }
		public int likes { get; set; }
		public int dislikes { get; set; }
	}
}

