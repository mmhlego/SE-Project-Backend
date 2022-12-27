using System;
namespace MyOnlineShop.Models.apimodel
{
    public class customersModel
    {
        public int customersPerPage { get; set; }
        public int page { get; set; }
        public List<customerModel> customers { get; set; }

    }


    public class customerreqModel
    {
        public string address { get; set; }
        public int balance { get; set; }
       

    }
    public class customerModel
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string email{ get; set; }
        public string profileImage { get; set; }
        public DateTime birthDate { get; set; }
        public string address { get; set; }
        public double balance { get; set; }
        public bool restricted { get; set; }
    }

    public class likeModel
    {
        public bool like { get; set; }
    }
}

