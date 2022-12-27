using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Models.apimodel;
using NuGet.Protocol.Plugins;
using System;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Http.Results;
using Microsoft.Win32;
using MyOnlineShop.Models;

namespace MyOnlineShop.Controllers
{

    public class AuthenticationController : ControllerBase
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _context.users.FirstOrDefault(u => u.UserName == loginModel.username && u.Password == loginModel.password);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
               
                new Claim(ClaimTypes.Role, user.AccessLevel)
            };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                
                HttpContext.SignInAsync(principal);

                var status = new Dictionary<string, string>() { { "status", "success" } };
                return Ok(status);
            }
        }
        
        [HttpPost]
        [Route("auth/register")]
        public IActionResult registermethod([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _context.users.SingleOrDefault(u => u.UserName == registerModel.username);
            if (user !=null)
            {
                ModelState.AddModelError("UserName", "This UserName Has been registered Already");               
            }
            
            User user1= new User()
            {
              UserName = registerModel.username,
              Password = registerModel.password,    
              AccessLevel = "customer",
              BirthDate= registerModel.birthDate,

            };
           
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

