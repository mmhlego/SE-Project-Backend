using System;
namespace MyOnlineShop.Models.apimodel
{
    public class Sellers
    {
        public int sellersPerPage { get; set; }
        public int page { get; set; }
        public List<apimodel.Seller> sellers { get; set; }
       
    }
    public class SellerpagePutMethodRequest
    {
        public string address { get; set; }
        public string information { get; set; }
    }
    public class Seller
    {  public Guid id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string address { get; set; }
        public string information { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }
       }


  
    }

