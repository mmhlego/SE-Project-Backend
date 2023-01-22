using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Models.apimodel;
using System.Security.Claims;
using MyOnlineShop.Models;
using MyOnlineShop.Services;

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
			Logger logger = new Logger(_context);
			if (!ModelState.IsValid)
			{
				logger.LoggerFunc("auth/login", loginModel, BadRequest(ModelState), User);
				return BadRequest(ModelState);
			}
			var user = _context.users.FirstOrDefault(u => u.UserName == loginModel.username && u.Password == loginModel.password);
			if (user == null)
			{
				logger.LoggerFunc("auth/login", 
						loginModel, new Dictionary<string, string>() { { "status", "Failed" } }, User);
				return Ok(new Dictionary<string, string>() { { "status", "Failed" } });
			}
			else if (user.Restricted)
			{
				logger.LoggerFunc("auth/login", 
						loginModel, new Dictionary<string, string>() { { "status", "Restricted" } }, User);
				return Ok(new Dictionary<string, string>() { { "status", "Restricted" } });
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

				 logger.LoggerFunc("auth/login", 
						loginModel, new Dictionary<string, string>() { { "status", "Success" } }, User);
				return Ok(new Dictionary<string, string>() { { "status", "Success" } });
			}
		}

		[HttpPost]
		[Route("auth/register")]
		public ActionResult registermethod([FromBody] RegisterModel registerModel)
		{
            Logger logger = new Logger(_context);
            try
			{
				if (!ModelState.IsValid)
				{
					logger.LoggerFunc("auth/register", 
						registerModel, StatusCode(StatusCodes.Status400BadRequest), User);
					return StatusCode(StatusCodes.Status400BadRequest);
				}
				var status = new Dictionary<string, string>();
				var username = _context.users.SingleOrDefault(u => u.UserName == registerModel.username);
				var email = _context.users.SingleOrDefault(u => u.Email == registerModel.email);
				var phone = _context.users.SingleOrDefault(u => u.PhoneNumber == registerModel.phoneNumber);
				if (username != null)
				{
					ModelState.AddModelError("UserName", "This UserName Has been registered Already");
					status = new Dictionary<string, string>() { { "status", "Exists" } };
				}
				else if (email != null)
				{
					ModelState.AddModelError("Email", "This Email Has been registered Already");
					status = new Dictionary<string, string>() { { "status", "Exists" } };
				}
				else if (phone != null)
				{
					ModelState.AddModelError("PhoneNumber", "This PhoneNumber Has been registered Already");
					status = new Dictionary<string, string>() { { "status", "Exists" } };
				}
				else
				{
					User user1 = new User()
					{
						ID = Guid.NewGuid(),
						AccessLevel = registerModel.type,
						UserName = registerModel.username,
						Password = registerModel.password,
						FirstName = registerModel.firstName,
						LastName = registerModel.lastName,
						PhoneNumber = registerModel.phoneNumber,
						Email = registerModel.email,
						BirthDate = registerModel.birthDate,
						ImageUrl = "",
						IsApproved = false,
						Restricted = false
					};
					_context.users.Add(user1);
					_context.SaveChanges();
					verificationCodeIn(user1.UserName);
					if (user1 == null)
					{
						status = new Dictionary<string, string>() { { "status", "Failed" } };
					}
					status = new Dictionary<string, string>() { { "status", "Success" } };
				}
				logger.LoggerFunc("auth/register", 
						registerModel, status, User);
				return Ok(status);
			}
			catch
			{
				logger.LoggerFunc("auth/register", 
						registerModel, StatusCode(StatusCodes.Status500InternalServerError), User);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		public void verificationCodeIn(string name)
		{

			var GuidKey = Guid.NewGuid();

			Verification VerGen = new Verification()
			{
				ID = GuidKey,
				UserName = name,
				ValidTime = DateTime.Now.AddHours(12)
			};
			_context.verification.Add(VerGen);
			_context.SaveChanges();
		}

		[HttpGet]
		[Route("auth/verify")]
		public ActionResult verify(Guid VerificationCode)
		{
			try
			{
				var status = new Dictionary<string, string>();
				User UserApprove = new User();
				var VerKey = _context.verification.SingleOrDefault(p => p.ID == VerificationCode);
				if (VerKey != null)
				{
					if (VerKey.ValidTime >= DateTime.Now)
					{

						var userId = _context.users.SingleOrDefault(l => l.UserName == VerKey.UserName);
						userId.IsApproved = true;
						_context.SaveChanges();
						status = new Dictionary<string, string>() { { "status", "Success" } };
					}
					else
					{
						status = new Dictionary<string, string>() { { "status", "Expired" } };
					}
				}
				else
				{
					ModelState.AddModelError("Invalid", "This Verification Code Not Valid");
					status = new Dictionary<string, string>() { { "status", "Invalid" } };
				}
				return Ok(status);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		[HttpGet]
		[Route("auth/logout")]
		public IActionResult Logout()
		{
			var status = new Dictionary<string, string>();
			try
			{

				HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				status = new Dictionary<string, string>() { { "status", "Success" } };
				return Ok(status);
			}
			catch
			{

				status = new Dictionary<string, string>() { { "status", "Failed" } };
				return Ok(status);
			}



		}
	}
}

