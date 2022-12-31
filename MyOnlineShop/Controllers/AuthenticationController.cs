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
using System.Linq;

namespace MyOnlineShop.Controllers
{

    public class AuthenticationController : ControllerBase
    {
        private MyShopContext _context;
        public AuthenticationController(MyShopContext context)
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
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.AccessLevel)
                };

                var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);
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
            var id = Guid.NewGuid();
            User u = new User()
            {
                ID = id,
                AccessLevel = "admin",
                BirthDate = DateTime.Today,
                Email = "fwe",
                FirstName = "wedw",
                ImageUrl = "gedger",
                LastName = "freer",
                PhoneNumber = "er",
                IsApproved = true,
                Password = "4",
                Restricted = false,
                UserName = "4",



            };
            Seller s = new Seller()
            {
                Address = "hhh",
                dislikes = 1,
                likes = 1,
                ID = Guid.NewGuid(),
                Information = "hh",
                UserId = id

            };
            _context.users.Add(u);
            _context.SaveChanges();
            // _context.sellers.Add(s);
            _context.SaveChanges();

            return NoContent();
        }
        [HttpGet]
        [Route("auth/verify")]
        public IActionResult verify(int VerificationCode)
        {



            var status = new Dictionary<string, string>() { { "status", "success" } };
            return Ok(status);

        }


    }
}

