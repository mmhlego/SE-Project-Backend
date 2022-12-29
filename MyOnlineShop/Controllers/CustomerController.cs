using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using MyOnlineShop.Models.apimodel;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using System.Data;

namespace MyOnlineShop.Controllers
{
	public class CustomerController : ControllerBase
	{
		private readonly MyShopContext _context;


		public CustomerController(MyShopContext context)
		{
			_context = context;
		}

		[HttpGet]
		[Route("Customers")]
		public ActionResult<IEnumerable<customersModel>> CustomersGet([FromQuery] int Page, [FromQuery] int CustomersPerPage)
		{
			try
			{
				var customers = new customersModel
				{
					page = Page,
					customersPerPage = CustomersPerPage,
					customers = _context.customer
							.Skip((Page - 1) * CustomersPerPage)
							.Take(CustomersPerPage).Select(u => new customerModel
							{
								id = u.UserId,
								username = u.user.UserName,
								firstName = u.user.FirstName,
								lastName = u.user.LastName,
								phoneNumber = u.user.PhoneNumber,
								email = u.user.Email,
								profileImage = u.user.ImageUrl,
								birthDate = u.user.BirthDate,
								restricted = u.user.Restricted,
								address = u.Address,
								balance = u.Balance
							}).ToList()
				};
				if (customers == null)
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				return Ok(customers);

			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}




		[HttpGet]
		[Route("Customers/{id}")]
		public ActionResult<IEnumerable<customerModel>> EachCustomer(Guid id)
		{
			try
			{
				var user = _context.users.SingleOrDefault(f => f.ID == id);
				var CustomerId = _context.customer.SingleOrDefault(p => p.UserId == user.ID);
				customerModel schema = new customerModel()
				{
					id = CustomerId.UserId,
					username = CustomerId.user.UserName,
					firstName = CustomerId.user.FirstName,
					lastName = CustomerId.user.LastName,
					phoneNumber = CustomerId.user.PhoneNumber,
					email = CustomerId.user.Email,
					profileImage = CustomerId.user.ImageUrl,
					birthDate = CustomerId.user.BirthDate,
					restricted = CustomerId.user.Restricted,
					address = CustomerId.Address,
					balance = CustomerId.Balance
				};
				if (CustomerId == null)
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				return Ok(schema);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		[HttpPut]
		[Route("Customers/{id}")]
		public ActionResult<IEnumerable<customerModel>> EachCustomerPut(Guid id, [FromQuery] customerreqModel custupdate)
		{
			try
			{
				customerModel custput = new customerModel();
				var CustomerId = _context.customer.SingleOrDefault(p => p.UserId == id);
				if (CustomerId == null)
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}
				else
				{
					custput = new customerModel()
					{
						id = id,
						username = CustomerId.user.UserName,
						firstName = CustomerId.user.FirstName,
						lastName = CustomerId.user.LastName,
						phoneNumber = CustomerId.user.PhoneNumber,
						email = CustomerId.user.Email,
						profileImage = CustomerId.user.ImageUrl,
						birthDate = CustomerId.user.BirthDate,
						restricted = CustomerId.user.Restricted,
						address = custupdate.address,
						balance = custupdate.balance
					};
					_context.SaveChanges();
					return Ok(custput);
				}

				if (!ModelState.IsValid)
				{
					return StatusCode(StatusCodes.Status400BadRequest);
				}
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


	}
}
