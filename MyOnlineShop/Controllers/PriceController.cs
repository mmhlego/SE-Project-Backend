using System;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyOnlineShop.Data;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;


namespace MyOnlineShop.Controllers
{
    public class PriceController: ControllerBase
    {
        private MyShopContex _context;
        public PriceController(MyShopContex context)
        {
            _context = context;

        }


        [HttpGet]
        [Route("prices/")]
        public ActionResult GetPrices( Guid sellerId = default(Guid), Guid productId = default(Guid), int pricesPerPage = 50, int page =1, double priceFrom = 0,double priceTo =100000000000, bool available = true) {
         //   try {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                else
                {
                    var prices = _context.productPrices.ToList();
                if (sellerId != default(Guid)) {
                    prices = prices.Where(p => p.SellerID == sellerId).ToList();

                }
                if (productId != default(Guid))
                {
                    prices = prices.Where(p => p.ProductID == productId).ToList();
                }
                if(available == true)
                {
                    prices = prices.Where(p => p.Amount > 0).ToList();

                }
                else
                {
                    prices = prices.Where(p => p.Amount == 0).ToList();
                }
                    prices = prices.Where(p => p.Price >= priceFrom && p.Price < priceTo).ToList();

                if (prices != null)
                {

                    if ((page * pricesPerPage) - pricesPerPage < prices.Count)
                    {
                        if (page * pricesPerPage > prices.Count)
                        {
                            prices = prices.GetRange((page * pricesPerPage) - pricesPerPage, prices.Count);

                        }
                        else
                        {
                            prices = prices.GetRange((page * pricesPerPage) - pricesPerPage, page * pricesPerPage);

                        }
                    }
                }

                    List<priceModel> priceModels = new List<priceModel>();
                foreach (ProductPrice price1 in prices)
                    {
                        Seller seller1 = _context.sellers.SingleOrDefault(s => s.ID == price1.SellerID);
                        User user = _context.users.SingleOrDefault(u => u.ID == seller1.UserId);
                        SellerSchema s = new SellerSchema()
                        {
                            id = seller1.ID,
                            address = seller1.Address,
                            likes = seller1.likes,
                            dislikes = seller1.dislikes,
                            image = user.ImageUrl
                        };
                        priceModel priceModel = new priceModel() {
                            id = price1.ID,
                            productId = price1.ProductID,
                            price = price1.Price,
                            amount = price1.Amount,
                            discount = price1.Discount,
                            priceHistory = price1.PriceHistory,
                            Seller = s
                       
                        };
                        priceModels.Add(priceModel);

                    }
               
                  

                    ProductPrices p = new ProductPrices()
                    {
                        page = page,
                        pricePerPage = pricesPerPage,
                        prices = priceModels
                    };
                    return Ok(p);
                }
          //  }
           // catch { return StatusCode(StatusCodes.Status500InternalServerError); }
        }



[HttpPost]
        [Route("prices/")]
        public ActionResult PostPrices([FromBody] PostPrice p1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    ProductPrice p = new ProductPrice();
                    return Ok(p);
                }
            }
            catch { return StatusCode(StatusCodes.Status500InternalServerError); }
        }


        [HttpGet]
        [Route("prices/{id:Guid}")]
        public ActionResult Getaprice(Guid id)
        {
            try
            {   
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                else
                {
                    var price1 = _context.productPrices.SingleOrDefault(p => p.ID == id);
                    var ss = _context.sellers.SingleOrDefault(s => s.ID == price1.SellerID);
                    var user = _context.users.SingleOrDefault(s => s.ID == ss.UserId);
                    SellerSchema schema = new SellerSchema()
                    {
                        information = ss.Information,
                        address = ss.Address,
                        id = ss.ID,
                        dislikes = ss.dislikes,
                        likes = ss.likes,
                        image = user.ImageUrl,
                        name = user.FirstName+" "+user.LastName
                    };
                    priceModel priceModel = new priceModel()
                    {
                        id = price1.ID,
                        productId = price1.ProductID,
                        price = price1.Price,
                        amount = price1.Amount,
                        discount = price1.Discount,
                        priceHistory = price1.PriceHistory,
                        Seller = schema

                    };
                    return Ok(priceModel);
                }
            }
            catch { return StatusCode(StatusCodes.Status500InternalServerError); }

        }



        [HttpPut]
        [Route("prices/")]
        public ActionResult PutPrices(int id, [FromBody] PutPrice p1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    ProductPrice p = new ProductPrice();
                    return Ok(p);
                }
            }
            catch { return StatusCode(StatusCodes.Status500InternalServerError); }
        }


        [HttpDelete]
        [Route("prices/{id:Guid}")]
        public ActionResult DeletePrice(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    ProductPrice p = new ProductPrice();
                    return Ok(p);
                }
            }
            catch { return StatusCode(StatusCodes.Status500InternalServerError); }
        }



    }
}

