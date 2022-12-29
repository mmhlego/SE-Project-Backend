using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Models.apimodel;
using System.Security.Claims;
using MyOnlineShop.Models;

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
				return StatusCode(StatusCodes.Status400BadRequest);
			}
            var status = new Dictionary<string, string>();
            var username = _context.users.SingleOrDefault(u => u.UserName == registerModel.username);
            var email = _context.users.SingleOrDefault(u => u.Email == registerModel.email);
            if (username != null)
			{
				ModelState.AddModelError("UserName", "This UserName Has been registered Already");
                status = new Dictionary<string, string>() { { "status", "Exists" } };
            }/*else if(email != null)
			{
                ModelState.AddModelError("Email", "This Email Has been registered Already");
            }*/
			else
			{
				User user1 = new User()
				{
                    AccessLevel = registerModel.type,
                    UserName = registerModel.username,
					Password = registerModel.password,
					FirstName = registerModel.firstName,
					LastName = registerModel.lastName,
					PhoneNumber = registerModel.phoneNumber,
					Email = registerModel.email,
					BirthDate = registerModel.birthDate
				};
				if(user1 == null)
				{
                    status = new Dictionary<string, string>() { { "status", "Failed" } };
                }
				status = new Dictionary<string, string>() { { "status", "Success" } };
			}
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

