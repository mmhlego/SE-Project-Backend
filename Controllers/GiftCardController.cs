using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;




namespace MyOnlineShop.Controllers
{
    public class GiftCardController : ControllerBase
    {
        GiftCard[] giftcards = new GiftCard[999];

        public IEnumerable<GiftCard> GetAllGiftCarts()
        {
            // Get: GiftCarts
            return giftcards;
        }
        public ActionResult GetGiftCart(Guid id)
        {
            var giftcart = giftcards.FirstOrDefault((p) => p.Id == id);
            if (giftcart == null)
            {
                return NotFound();
            }
            return Ok(giftcart);
        }
    }
}
