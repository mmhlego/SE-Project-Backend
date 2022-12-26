using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;




namespace MyOnlineShop.Controllers
{
    public class CartController : ControllerBase
    {
        List<Cart> carts;

        public IEnumerable<Cart> GetAllCarts()
        {
            // Get: Carts
            return carts;
        }
        public ActionResult GetCart(Guid id)
        {
            var cart = carts.FirstOrDefault((p) => p.ID == id);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }
    }
}
