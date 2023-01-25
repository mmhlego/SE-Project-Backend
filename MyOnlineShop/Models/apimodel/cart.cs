namespace MyOnlineShop.Models.apimodel
{
	public class tokenresmodel
	{
		public Guid id { get; set; }
		public DateTime expirationDate { get; set; }
		public string discount { get; set; }
		public Boolean isEvent { get; set; }
	}
	public class eachCart
	{
		public Guid id { get; set; }
		public Guid customerId { get; set; }
		public List<eachproduct> products { get; set; }
		public string status { get; set; }
		public string description { get; set; }
		public DateTime updateDate { get; set; }
		public double totalprice { get; set; }

	}

	public class eachproduct
	{
		public Guid productId { get; set; }
		public int amount { get; set; }

		//public string description { get; set; }

	}



	public class token
	{
		public string status { get; set; }
		public eachCart cart { get; set; }
	}
}

//{
//    "cartsPerPage": 0,
//  "page": 0,
//  "carts": [
//    {
//        "id": "string",
//      "customerId": "string",
//      "products": [
//        {
//            "productId": "string",
//          "amount": 0
//        }
//      ],
//      "status": "Filling",
//      "description": "string",
//      "updateDate": "string"
//    }
//  ]
//}