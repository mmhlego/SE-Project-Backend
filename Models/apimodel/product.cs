using System;
namespace MyOnlineShop.Models.apimodel
{
    public class Product
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }
    }
   
    public class ProductPagePostRequestModel
    {
        public string name { get; set; }
        public string category { get; set; }
        public string image { get; set; }
        public string description { get; set; }
    }
    public class ProductPageGetRequestModel
    {
        public int productsPerPage { get; set; }
        public int page { get; set; }
        public double priceFrom { get; set; }
        public double priceTo { get; set; }
        public Boolean available { get; set; }
        public Guid catagory { get; set; }
        
    }

    public class Products
    {
        public int productsPerPage { get; set; }
        public int page { get; set; }
        public List<ProductPagePostRequestModel> products { get; set; }

    }
}

