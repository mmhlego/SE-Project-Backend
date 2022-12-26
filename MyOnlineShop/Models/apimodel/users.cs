using System;
namespace MyOnlineShop.Models.apimodel
{
    public class usersModel
    {
        public int usersPerPage { get; set; }
        public int page { get; set; }
        public List<userModel> users { get; set; }

    }
    public class userModel
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string profileImage { get; set; }
        public DateTime birthDate { get; set; }
        public string accessLevel { get; set; }
        public bool restricted { get; set; }
    }



    public class userreqModel
    { 
        public string phoneNumber { get; set; }
        public string email { get; set; }
  
        public string accessLevel { get; set; }
        public bool restricted { get; set; }
    }

    public class tokensModel
    {
        public int tokensPerPage { get; set; }
        public int page { get; set; }
        public List<GiftCard> tokens { get; set; }

    }



    public class tokenreqModel
    { 
        public DateTime ExpirationDate { get; set; }
        public string Discount { get; set; }
        public Boolean IsEvent { get; set; }


    }



}

