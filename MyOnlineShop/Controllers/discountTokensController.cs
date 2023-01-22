using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Services;
using MyOnlineShop.Models.apimodel;
using MyOnlineShop.Models;

namespace MyOnlineShop.Controllers
{
    public class discountTokensController : ControllerBase
    {

        private MyShopContext _context;
        public discountTokensController(MyShopContext context)
        {
            _context = context;

        }

        [HttpGet]
        [Route("discountTokens/{id}/Validate")]
        public ActionResult discountTokenGet(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var s = new Dictionary<string, string>();
                var token = _context.tokens.Where(t => t.Id == id).Single();

                if (token != null)
                {
                    if (DateTime.Now < token.ExpirationDate)
                    {
                        s = new Dictionary<string, string>() { { "status", "Valid" } };
                    }
                    else
                    {
                        s = new Dictionary<string, string>() { { "status", "InValid" } };
                    }

                }
                else
                {
                    s = new Dictionary<string, string>() { { "status", "InValid" } };

                }

                return Ok(s);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("discountTokens/{id}/use")]

        public ActionResult discountTokenPost(Guid id, Guid cartId)
        {

            Logger logger = new Logger(_context);
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
            {
                logger.LoggerFunc($"discountTokens/{id}/use",
                                cartId, Unauthorized(), User);
                return Unauthorized();
            }
            var userId = _context.users.SingleOrDefault(u => u.UserName == username).ID;
            var customer = _context.customer.SingleOrDefault(c => c.UserId == userId);
            if (customer == null)
            {
                logger.LoggerFunc($"discountTokens/{id}/use",
                                cartId, Forbid(), User);
                return Forbid();
            }
            Cart cart = new Cart();
            if (cartId == default(Guid))
            {
                cart = _context.cart.SingleOrDefault(c => c.CustomerID == customer.ID && c.Status.ToLower() == "filling");

            }
            else
            {

                cart = _context.cart.SingleOrDefault(C => C.ID == cartId);
            }
            var token1 = _context.tokens.SingleOrDefault(t => t.Id == id);

            if (cart == null)
            {
                logger.LoggerFunc($"discountTokens/{id}/use",
                                cartId, NotFound(), User);
                return NotFound();
            }
            if (cart.TotalPrice == 0)
            {
                logger.LoggerFunc($"discountTokens/{id}/use",
                                cartId, BadRequest(), User);
                return BadRequest();
            }


            var status = "Invalid";
            if (token1 != null && DateTime.Now <= token1.ExpirationDate)

            {
                status = "Valid";
                string[] t = token1.Discount.Split(new char[] { '_' });
                double a = Convert.ToDouble(t[1]);
                if (t[0] == "AMOUNT")
                {
                    cart.TotalPrice = cart.TotalPrice - a;
                }
                else if (t[0] == "PERCENT")
                {
                    cart.TotalPrice = cart.TotalPrice - (cart.TotalPrice * a / 100);
                }
                cart.UpdateDate = DateTime.Now;
                _context.Update(cart);

                _context.SaveChanges();
            }
            var orders = _context.orders.Where(o => o.CartID == cart.ID).ToList();
            var ps = new List<eachproduct>();
            foreach (var o in orders)
            {
                var product = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID);

                eachproduct p = new eachproduct()
                {
                    productId = product.ProductID,
                    amount = o.Amount

                };
                ps.Add(p);
            }
            eachCart eachCart = new eachCart()
            {
                customerId = cart.CustomerID,
                description = cart.Description,
                id = cart.ID,
                products = ps,
                status = "Filling",
                updateDate = cart.UpdateDate


            };
            token t1 = new token()
            {
                status = status,
                cart = eachCart
            };
            logger.LoggerFunc($"discountTokens/{id}/use",
                                cartId, t1, User);
            return Ok(t1);



        }
    }
}

