﻿using System;
namespace MyOnlineShop.Models.apimodel
{
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class RegisterModel
    {
        public Guid id { get; set; }
        public string type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

        public DateTime birthDate { get; set; }

    }
}