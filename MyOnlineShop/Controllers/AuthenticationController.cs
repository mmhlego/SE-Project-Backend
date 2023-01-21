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
			if (!ModelState.IsValid)
			{
                Logger.LoggerFunc("auth/login", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        loginModel, BadRequest(ModelState));
                return BadRequest(ModelState);
			}
			var user = _context.users.FirstOrDefault(u => u.UserName == loginModel.username && u.Password == loginModel.password);
			if (user == null)
			{
                Logger.LoggerFunc("auth/login", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        loginModel, new Dictionary<string, string>() { { "status", "Failed" } });
                return Ok(new Dictionary<string, string>() { { "status", "Failed" } });
			}
			else if (user.Restricted)
			{
                Logger.LoggerFunc("auth/login", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        loginModel, new Dictionary<string, string>() { { "status", "Restricted" } });
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

           //     Logger.LoggerFunc("auth/login", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
            //            loginModel, new Dictionary<string, string>() { { "status", "Success" } });
                return Ok(new Dictionary<string, string>() { { "status", "Success" } });
			}
		}

		[HttpPost]
		[Route("auth/register")]
		public ActionResult registermethod([FromBody] RegisterModel registerModel)
		{
			try
			{
				if (!ModelState.IsValid)
				{
                    Logger.LoggerFunc("auth/register", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        registerModel, StatusCode(StatusCodes.Status400BadRequest));
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
                Logger.LoggerFunc("auth/register", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        registerModel, status);
                return Ok(status);
			}
			catch
			{
                Logger.LoggerFunc("auth/register", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        registerModel, StatusCode(StatusCodes.Status500InternalServerError));
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
		public ActionResult<IEnumerable<userModel>> verify(Guid VerificationCode)
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
            try {
                
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                status = new Dictionary<string, string>() { { "status", "Success" } };
				return Ok(status);
            } catch {

                status = new Dictionary<string, string>() { { "status", "Failed" } };
                return Ok(status);
            }
            
            
            
        }
    }
}

