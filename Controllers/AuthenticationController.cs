using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Models.apimodel;
using System;
using System.Web.Helpers;
using System.Web.Http.Results;

namespace MyOnlineShop.Controllers
{
   
    public class AuthenticationController: ControllerBase
    {
        private MyShopContex _context;
        public AuthenticationController(MyShopContex context)
        {
            _context = context;

        }

        [HttpPost]
        [Route("auth/login")]
        public IActionResult loginmethod([FromBody] LoginModel loginModel)
        {
            var user = _context.users.FirstOrDefault(u => u.UserName==loginModel.username && u.Password== loginModel.password);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var status = new Dictionary<string, string>() { {"status","success" } };
                return Ok(status);
            }
        }
        public IActionResult register(LoginModel loginModel) {



            return Ok();
        }

        [HttpPost]
        [Route("auth/register")]
        public IActionResult registermethod([FromBody] RegisterModel registerModel)
        {    
           
        
        
                var status = new Dictionary<string, string>() { { "status", "success" } };
                return Ok(status);
            
        }

        [HttpGet]
        [Route("auth/verify")]
        public IActionResult verify(int VerficationCode)
        {



            var status = new Dictionary<string, string>() { { "status", "success" } };
            return Ok(status);

        }


    }
}

