using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.OpenApi.Validations.Rules;
using MyOnlineShop.Data;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Mvc;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;


namespace MyOnlineShop.Controllers
{
    public class PriceController : ControllerBase
    {
        private MyShopContext _context;
        public PriceController(MyShopContext context)
        {
            _context = context;

        }


        [HttpGet]
        [Route("prices/")]
        public ActionResult GetPrices(Guid sellerId = default(Guid), Guid productId = default(Guid), int pricesPerPage = 50, int page = 1, double priceFrom = 0, double priceTo = 100000000000, bool available = true)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                else
                {
                    var prices = _context.productPrices.ToList();
                    if (sellerId != default(Guid))
                    {
                        prices = prices.Where(p => p.SellerID == sellerId).ToList();

                    }
                    if (productId != default(Guid))
                    {
                        prices = prices.Where(p => p.ProductID == productId).ToList();
                    }
                    if (available == true)
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
                        priceModel priceModel = new priceModel()
                        {
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
            }
            catch { return StatusCode(StatusCodes.Status500InternalServerError); }
        }



        [HttpPost]
        [Authorize]
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

                    var username = User.FindFirstValue(ClaimTypes.Name);
                    var userId = _context.users.SingleOrDefault(u => u.UserName == username).ID;
                    Seller seller = new Seller();
                    string accesslevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                    var product = _context.Products.SingleOrDefault(p => p.ID == p1.ProductID);

                    if (product == null)
                    {
                        return NotFound();
                    }
                    if (accesslevel == "seller" || accesslevel == "admin")

                    {
                        if (accesslevel == "admin")
                        {
                            seller = _context.sellers.SingleOrDefault(s => s.ID == p1.SellerID);
                        }
                        if (accesslevel == "seller")
                        {
                            seller = _context.sellers.SingleOrDefault(s => s.UserId == userId);
                        }
                        var checkseller = _context.productPrices.SingleOrDefault(p => p.ProductID == p1.ProductID && p.SellerID == seller.ID);
                        var user = _context.users.SingleOrDefault(u => u.ID == seller.UserId);
                        if (product != null && checkseller == null)
                        {

                            var productPrice = new ProductPrice()
                            {
                                Price = p1.Price,
                                PriceHistory = "[]",
                                Amount = p1.Amount,
                                Discount = p1.Discount,
                                SellerID = p1.SellerID,
                                ProductID = p1.ProductID,
                                ID = Guid.NewGuid()

                            };
                            _context.productPrices.Add(productPrice);
                            _context.SaveChanges();
                            SellerSchema s = new SellerSchema()
                            {
                                address = seller.Address,
                                likes = seller.likes,
                                dislikes = seller.dislikes,
                                id = seller.ID,
                                image = user.ImageUrl,
                                information = seller.Information,
                                name = user.FirstName + " " + user.LastName,
                                restricted = user.Restricted
                            };
                            priceModel priceModel = new priceModel()
                            {
                                id = productPrice.ID,
                                productId = productPrice.ProductID,
                                price = productPrice.Price,
                                amount = productPrice.Amount,
                                discount = productPrice.Discount,
                                priceHistory = productPrice.PriceHistory,
                                Seller = s

                            };

                            return Ok(priceModel);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }

                    else
                    {
                        return Unauthorized();
                    }

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
                        name = user.FirstName + " " + user.LastName
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
        [Route("prices/{id:Guid}")]
        [Authorize]
        public ActionResult PutPrices(Guid id, [FromBody] PutPrice p1)
        {


            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                //  Guid userId =User.FindFirstValue("userId");
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userId = _context.users.SingleOrDefault(u => u.UserName == username).ID;
                var accesslevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                ProductPrice productPrice = new ProductPrice();

                if (accesslevel == "seller")
                {
                    var seller = _context.sellers.SingleOrDefault(s => s.UserId == userId);

                    var checkseller = _context.productPrices.SingleOrDefault(p => p.ID == id && p.SellerID == seller.ID);
                    var user = _context.users.SingleOrDefault(u => u.ID == seller.UserId);


                    if (checkseller != null)
                    {
                        checkseller.Price = p1.Price;
                        checkseller.Amount = p1.Amount + checkseller.Amount;
                        checkseller.Discount = p1.Discount;

                        _context.Update(checkseller);

                        _context.SaveChanges();
                        productPrice = checkseller;


                        SellerSchema s = new SellerSchema()
                        {
                            address = seller.Address,
                            likes = seller.likes,
                            dislikes = seller.dislikes,
                            id = seller.ID,
                            image = user.ImageUrl,
                            information = seller.Information,
                            name = user.FirstName + " " + user.LastName,
                            restricted = user.Restricted
                        };
                        priceModel priceModel = new priceModel()
                        {
                            id = id,
                            productId = productPrice.ProductID,
                            price = productPrice.Price,
                            amount = productPrice.Amount,
                            discount = productPrice.Discount,
                            priceHistory = productPrice.PriceHistory,
                            Seller = s

                        };

                        return Ok(priceModel);
                    }
                    else
                    {


                        return Forbid();
                    }
                }
                else
                {

                    if (accesslevel == "storekeeper")
                    {

                        productPrice = _context.productPrices.SingleOrDefault(c => c.ID == id);
                        var seller = _context.sellers.SingleOrDefault(s => s.ID == productPrice.SellerID);
                        var user = _context.users.SingleOrDefault(u => u.ID == seller.UserId);
                        if (productPrice == null)
                        {
                            return NotFound();
                        }
                        productPrice.Price = p1.Price;
                        productPrice.Amount = p1.Amount + productPrice.Amount;
                        productPrice.Discount = p1.Discount;


                        _context.Update(productPrice);

                        _context.SaveChanges();
                        SellerSchema s = new SellerSchema()
                        {
                            address = seller.Address,
                            likes = seller.likes,
                            dislikes = seller.dislikes,
                            id = seller.ID,
                            image = user.ImageUrl,
                            information = seller.Information,
                            name = user.FirstName + " " + user.LastName,
                            restricted = user.Restricted
                        };
                        priceModel priceModel = new priceModel()
                        {
                            id = id,
                            productId = productPrice.ProductID,
                            price = productPrice.Price,
                            amount = productPrice.Amount,
                            discount = productPrice.Discount,
                            priceHistory = productPrice.PriceHistory,
                            Seller = s

                        };

                        return Ok(priceModel);



                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

            }
            catch { return StatusCode(StatusCodes.Status500InternalServerError); }
        }


        [HttpDelete]
        [Authorize]
        [Route("prices/{id:Guid}")]
        public ActionResult DeletePrice(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var username = User.FindFirstValue(ClaimTypes.Name);
                var userId = _context.users.SingleOrDefault(u => u.UserName == username).ID;

                var accesslevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                var productPrice = _context.productPrices.SingleOrDefault(p => p.ID == id);
                if (productPrice == null)
                { return NotFound(); }

                if (accesslevel == "seller")
                {
                    var seller = _context.sellers.SingleOrDefault(s => s.UserId == userId);
                    var checkseller = _context.productPrices.SingleOrDefault(p => p.ID == id && p.SellerID == seller.ID);
                    var user = _context.users.SingleOrDefault(u => u.ID == seller.UserId);
                    if (checkseller != null)
                    {
                        checkseller.Amount = 0;

                        _context.Update(checkseller);
                        _context.SaveChanges();



                        SellerSchema s = new SellerSchema()
                        {
                            address = seller.Address,
                            likes = seller.likes,
                            dislikes = seller.dislikes,
                            id = seller.ID,
                            image = user.ImageUrl,
                            information = seller.Information,
                            name = user.FirstName + " " + user.LastName,
                            restricted = user.Restricted
                        };
                        priceModel priceModel = new priceModel()
                        {
                            id = id,
                            productId = productPrice.ProductID,
                            price = productPrice.Price,
                            amount = productPrice.Amount,
                            discount = productPrice.Discount,
                            priceHistory = productPrice.PriceHistory,
                            Seller = s

                        };


                        return Ok(priceModel);
                    }

                    else
                    {
                        return Forbid();
                    }

                }
                else
                {
                    if (accesslevel == "admin")
                    {
                        productPrice.Amount = 0;
                        _context.Update(productPrice);
                        _context.SaveChanges();
                        var seller = _context.sellers.SingleOrDefault(s => s.ID == productPrice.SellerID);
                        var user = _context.users.SingleOrDefault(u => u.ID == seller.UserId);
                        SellerSchema s = new SellerSchema()
                        {
                            address = seller.Address,
                            likes = seller.likes,
                            dislikes = seller.dislikes,
                            id = seller.ID,
                            image = user.ImageUrl,
                            information = seller.Information,
                            name = user.FirstName + " " + user.LastName,
                            restricted = user.Restricted
                        };
                        priceModel priceModel = new priceModel()
                        {
                            id = id,
                            productId = productPrice.ProductID,
                            price = productPrice.Price,
                            amount = productPrice.Amount,
                            discount = productPrice.Discount,
                            priceHistory = productPrice.PriceHistory,
                            Seller = s

                        };

                        return Ok(priceModel);



                    }
                    else
                    {
                        return Unauthorized();
                    }
                }


            }
            catch { return StatusCode(StatusCodes.Status500InternalServerError); }
        }



    }
}

