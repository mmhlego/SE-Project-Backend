using System;
namespace MyOnlineShop.Models.apimodel
{
    public class profileModel{

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


    public class putprofileModel
    {
       
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string profileImage { get; set; }
        public DateTime birthDate { get; set; }
        
    }




}

