namespace MyOnlineShop.Models.apimodel
{
    public class statsModel
    {
        public int allstatsPerPage { get; set; }
        public int page { get; set; }
        public List<stats> stats { get; set; }

    }
    public class stats
    {
        public Guid productId { get; set; }
        public Guid sellerId { get; set; }
        public DateTime date { get; set; }
        public int amount { get; set; }
        public double price { get; set; }
       
    }

}