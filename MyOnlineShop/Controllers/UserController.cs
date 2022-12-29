using Microsoft.AspNetCore.Mvc;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using MyOnlineShop.Data;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace MyOnlineShop.Controllers
{
	public class UserController : ControllerBase
	{
		private MyShopContext _context;
		public UserController(MyShopContext context)
		{
			_context = context;

		}


		[HttpGet]
		[Route("user/{id}")]
		public ActionResult GetUser(Guid id)
		{
			var user = _context.users.SingleOrDefault(u => u.ID == id);
			if (user == null)
			{
				return NotFound();
			}
			return Ok(user);
		}
	}
}
