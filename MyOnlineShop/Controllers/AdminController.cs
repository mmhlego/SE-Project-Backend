using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;
using System.Data;
using MyOnlineShop.Data;
using MyOnlineShop.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MyOnlineShop.Controllers
{
    public class AdminController : ControllerBase
    {
        private readonly MyShopContext _context;

        public AdminController(MyShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("admin/users")]
        public ActionResult<IEnumerable<usersModel>> userssget([FromQuery] int page, [FromQuery] int usersPerPage)
        {
            try
            {
                string username = User.FindFirstValue(ClaimTypes.Name);
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {
                    var lenght = _context.users.ToList().Count();
                    if ((page - 1) * usersPerPage > lenght)
                    {
                        page = (lenght / (usersPerPage));
                    }
                    if (page * usersPerPage > lenght && (page - 1) * usersPerPage < lenght)
                    {
                        usersPerPage = lenght - (page - 1) * usersPerPage;

                    }
                    if (usersPerPage > lenght)
                    {
                        page = 1;
                        usersPerPage = lenght;
                    }

                    var users = new usersModel
                    {
                        page = page,
                        usersPerPage = usersPerPage,
                        users = _context.users
                    .Skip((page - 1) * usersPerPage)
                    .Take(usersPerPage)
                    .Select(u => new userModel
                    {
                        id = u.ID,
                        username = u.UserName,
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        phoneNumber = u.PhoneNumber,
                        email = u.Email,
                        profileImage = u.ImageUrl,
                        birthDate = u.BirthDate,
                        accessLevel = u.AccessLevel,
                        isApproved = u.IsApproved,
                        restricted = u.Restricted
                    })
                    .ToList()
                    };
                    if (users.users == null)
                    {
                        return NotFound();
                    }

                    return Ok(users);
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpGet]
        [Route("admin/users/{id}")]
        public ActionResult eachuserget(Guid id)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }

                if (accessLevel == "admin")
                {
                    var userId = _context.users.SingleOrDefault(p => p.ID == id);
                    if (userId == null)
                    {
                        return NotFound();
                    }
                    var user = new userModel
                    {
                        id = userId.ID,
                        username = userId.UserName,
                        firstName = userId.FirstName,
                        lastName = userId.LastName,
                        phoneNumber = userId.PhoneNumber,
                        email = userId.Email,
                        profileImage = userId.ImageUrl,
                        birthDate = userId.BirthDate,
                        accessLevel = userId.AccessLevel,
                        isApproved = userId.IsApproved,
                        restricted = userId.Restricted
                    };
                    return Ok(user);
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpPut]
        [Route("admin/users/{id}")]
        public ActionResult userput(Guid id, [FromBody] userreqModel req)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {
                    var userId = _context.users.SingleOrDefault(p => p.ID == id);

                    if (userId == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        if (req.phoneNumber != null)
                        {
                            userId.PhoneNumber = req.phoneNumber;
                        }
                        if (req.email != null)
                        {
                            userId.Email = req.email;
                        }
                        if (req.accessLevel != null)
                        {
                            userId.AccessLevel = req.accessLevel;
                        }
                        if (req.accessLevel != null)
                        {
                            userId.Restricted = req.restricted;
                        }
                        _context.Update(userId);
                        _context.SaveChanges();
                        Logger.LoggerFunc(User.FindFirstValue(ClaimTypes.Name), "Put", "User_Created_By_ID");
                        userModel u = new userModel()
                        {
                            accessLevel = userId.AccessLevel,
                            birthDate = userId.BirthDate,
                            email = userId.Email,
                            firstName = userId.FirstName,
                            lastName = userId.LastName,
                            id = userId.ID,
                            phoneNumber = userId.PhoneNumber,
                            restricted = userId.Restricted,
                            profileImage = userId.ImageUrl,
                            username = userId.UserName,
                            isApproved = userId.IsApproved
                        };
                        return Ok(u);
                    }

                    if (!ModelState.IsValid)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




        [HttpDelete]
        [Route("admin/users/{id}")]
        public ActionResult userdelete(Guid id)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {

                    var userId = _context.users.SingleOrDefault(p => p.ID == id);
                    bool restricted = true;
                    if (userId == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        userId.Restricted = restricted;
                        if (userId.AccessLevel.ToLower() == "seller")
                        {
                            var productprices = _context.productPrices.Where(s => s.SellerID == userId.ID).ToList();
                            foreach (var p in productprices)
                            {
                                p.Amount = 0;
                                _context.productPrices.Update(p);
                                _context.SaveChanges();
                            }
                        }
                        _context.Update(userId);
                        _context.SaveChanges();

                        Logger.LoggerFunc(User.FindFirstValue(ClaimTypes.Name), "Delete", "Delete_User_by_ID");
                        userModel u = new userModel()
                        {
                            accessLevel = userId.AccessLevel,
                            birthDate = userId.BirthDate,
                            email = userId.Email,
                            firstName = userId.FirstName,
                            lastName = userId.LastName,
                            id = userId.ID,
                            phoneNumber = userId.PhoneNumber,
                            restricted = userId.Restricted,
                            profileImage = userId.ImageUrl,
                            username = userId.UserName,
                            isApproved = userId.IsApproved
                        };
                        return Ok(u);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpGet]
        [Route("admin/discountTokens")]
        public ActionResult<IEnumerable<tokensModel>> discountTokens(bool isEvent, bool expired, int page, int tokensPerPage)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {
                    var alltokens = _context.tokens.ToList();
                    if (isEvent == true || isEvent == false)
                    {
                        alltokens = alltokens.Where(P => P.IsEvent == isEvent).ToList();
                    }
                    if (expired == true || expired == false)
                    {
                        if (expired == true)
                        {
                            alltokens = alltokens.Where(p => p.ExpirationDate > DateTime.Now).ToList();

                        }
                        else
                        {
                            alltokens = alltokens.Where(p => p.ExpirationDate <= DateTime.Now).ToList();
                        }
                    }

                    var lenght = _context.tokens.ToList().Count();
                    if (lenght == 0)
                    {
                        return NotFound();
                    }
                    if ((page - 1) * tokensPerPage > lenght)
                    {
                        page = (lenght / (tokensPerPage));
                    }
                    if (page * tokensPerPage > lenght && (page - 1) * tokensPerPage < lenght)
                    {
                        tokensPerPage = lenght - (page - 1) * tokensPerPage;

                    }
                    if (tokensPerPage > lenght)
                    {
                        page = 1;
                        tokensPerPage = lenght;
                    }
                    var tokens = new tokensModel();

                    tokens = new tokensModel
                    {
                        page = page,
                        tokensPerPage = tokensPerPage,
                        tokens = alltokens
.Skip((page - 1) * tokensPerPage)
.Take(tokensPerPage)
.Select(u => new DiscountToken
{
Id = u.Id,
ExpirationDate = u.ExpirationDate,
Discount = u.Discount,
IsEvent = u.IsEvent
}).ToList()

                    };
                    if (tokens.tokens.Count == 0)
                    {
                        return NotFound();
                    }

                    return Ok(tokens);



                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpPost]
        [Route("admin/discountTokens")]
        public ActionResult discountTokenspost([FromBody] tokenreqModel tokenputter)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {
                    DiscountToken tokenput = new DiscountToken()
                    {
                        ExpirationDate = tokenputter.expirationDate,
                        Discount = tokenputter.discount,
                        IsEvent = tokenputter.isEvent,
                        Id = Guid.NewGuid()
                    };


                    _context.tokens.Add(tokenput);
                    _context.SaveChanges();
                    Logger.LoggerFunc(User.FindFirstValue(ClaimTypes.Name), "Post", "Create_DiscountToken");
                    tokenresmodel t = new tokenresmodel()
                    {
                        discount = tokenput.Discount,
                        id = tokenput.Id,
                        isEvent = tokenput.IsEvent,
                        expirationDate = tokenput.ExpirationDate
                    };
                    return Ok(t);

                    if (!ModelState.IsValid)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




        [HttpDelete]
        [Route("admin/discountTokens/{id}")]
        public ActionResult discountTokensdelete(Guid id)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {
                    var delToken = _context.tokens.SingleOrDefault(p => p.Id == id);
                    if (delToken == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        _context.tokens.Remove(delToken);
                        _context.SaveChanges();
                        tokenresmodel t = new tokenresmodel()
                        {
                            discount = delToken.Discount,
                            id = delToken.Id,
                            isEvent = delToken.IsEvent,
                            expirationDate = delToken.ExpirationDate
                        };
                        Logger.LoggerFunc(User.FindFirstValue(ClaimTypes.Name), "Delete", "Delete_DiscountToken_By_ID");
                        return Ok(t);

                    }



                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //[HttpGet]
        //[Route("admin/carts")]
        //public ActionResult<IEnumerable<cartModel>> admincarts(Guid? userId, bool? current, [FromQuery] int page, [FromQuery] int cartsPerPage)
        //{
        // try
        // {  
        // string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
        // if(accessLevel == null)
        //              {
        // return Unauthorized();
        //              }
        // if (accessLevel != "admin")
        //              {
        // return Forbid();
        //              }
        //              var c = _context.cart.ToList();
        //              if (current.HasValue)
        //              {
        // if(current.Value == true)
        //                  {
        // c = c.Where(c => c.Status == "Filling").ToList();
        //                  }
        //                  else
        //                  {
        //                      c = c.Where(c => c.Status != "Filling").ToList();
        //                  }
        //              }
        // if(userId != default(Guid))
        //              { var user = _context.customer.SingleOrDefault(c => c.UserId == userId);
        // if(user == null)
        //                  {
        // return NotFound();
        //                  }
        // c = c.Where(c => c.CustomerID == user.ID).ToList();
        //              }
        //              foreach (var cart in c)
        //              {
        // var orders = _context.orders.Where(o => o.CartID == cart.ID);
        // foreach(var o in orders)
        //                  {

        //                  }
        //              }

        //              {
        // var carts = new cartModel();
        // if (current.HasValue)
        // {
        // if (current.Value == true)
        // {
        // //current = True => status = Approved
        // if (userId.HasValue)
        // {  
        // carts = new cartModel
        // {    
        // page = page,
        // cartsPerPage = cartsPerPage,
        // carts = _context.cart.Where(d => d.Status == "Approved" && d.ID == userId)
        // .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
        // {
        // id = u.ID,
        // customerId = u.CustomerID,
        // products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
        // {
        // productId = l.ProductID,
        // amount = l.Amount
        // }).ToList(),
        // status = u.Status,
        // description = u.Description,
        // updateDate = u.UpdateDate
        // }).ToList()
        // };
        // }
        // else
        // {
        // carts = new cartModel
        // {
        // page = page,
        // cartsPerPage = cartsPerPage,
        // carts = _context.cart.Where(d => d.Status == "Approved")
        // .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
        // {
        // id = u.ID,
        // customerId = u.CustomerID,
        // products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
        // {
        // productId = l.ProductID,
        // amount = l.Amount
        // }).ToList(),
        // status = u.Status,
        // description = u.Description,
        // updateDate = u.UpdateDate
        // }).ToList()
        // };

        // }
        // }
        // else if (current.Value == false)
        // {
        // //current = fasle => status = Rejected
        // if (userId.HasValue)
        //                          {
        // var c = _context.cart.Where(d => d.Status != "Filling" && d.ID == userId).ToList();
        // var lenght = c.Count();
        //                              if (lenght == 0)
        //                              {
        //                                  return NotFound();
        //                              }
        //                              if ((page - 1) * cartsPerPage > lenght)
        //                              {
        //                                  page = (lenght / (cartsPerPage));
        //                              }
        //                              if (page * cartsPerPage > lenght && (page - 1) * cartsPerPage < lenght)
        //                              {
        //                                  cartsPerPage = lenght - (page - 1) * cartsPerPage;

        //                              }
        //                              if (cartsPerPage > lenght)
        //                              {
        //                                  page = 1;
        //                                  cartsPerPage = lenght;
        //                              }
        //                              carts = new cartModel
        // {
        // page = page,
        // cartsPerPage = cartsPerPage,
        // carts = c
        // .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
        // {
        // id = u.ID,
        // customerId = u.CustomerID,
        // products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
        // {
        // productId = l.ProductID,
        // amount = l.Amount
        // }).ToList(),
        // status = u.Status,
        // description = u.Description,
        // updateDate = u.UpdateDate
        // }).ToList()
        // };
        // }
        // else
        //                          {
        // var c = _context.cart.Where(d => d.Status != "Filling").ToList();
        //                              var lenght = c.Count();
        //                              if (lenght == 0)
        //                              {
        //                                  return NotFound();
        //                              }
        //                              if ((page - 1) * cartsPerPage > lenght)
        //                              {
        //                                  page = (lenght / (cartsPerPage));
        //                              }
        //                              if (page * cartsPerPage > lenght && (page - 1) * cartsPerPage < lenght)
        //                              {
        //                                  cartsPerPage = lenght - (page - 1) * cartsPerPage;

        //                              }
        //                              if (cartsPerPage > lenght)
        //                              {
        //                                  page = 1;
        //                                  cartsPerPage = lenght;
        //                              }
        //                              carts = new cartModel
        // {
        // page = page,
        // cartsPerPage = cartsPerPage,
        // carts = c
        // .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
        // {
        // id = u.ID,
        // customerId = u.CustomerID,
        // products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
        // {
        // productId = l.ProductID,
        // amount = l.Amount
        // }).ToList(),
        // status = u.Status,
        // description = u.Description,
        // updateDate = u.UpdateDate
        // }).ToList()
        // };

        // }
        // }
        // }
        // else
        // {
        // if (userId.HasValue)
        // {
        // var c = _context.cart.Where(d => d.ID == userId).ToList();
        //                          var lenght = c.Count();
        //                          if (lenght == 0)
        //                          {
        //                              return NotFound();
        //                          }
        //                          if ((page - 1) * cartsPerPage > lenght)
        //                          {
        //                              page = (lenght / (cartsPerPage));
        //                          }
        //                          if (page * cartsPerPage > lenght && (page - 1) * cartsPerPage < lenght)
        //                          {
        //                              cartsPerPage = lenght - (page - 1) * cartsPerPage;

        //                          }
        //                          if (cartsPerPage > lenght)
        //                          {
        //                              page = 1;
        //                              cartsPerPage = lenght;
        //                          }
        //                          carts = new cartModel
        // {
        // page = page,
        // cartsPerPage = cartsPerPage,
        // carts = c
        // .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
        // {
        // id = u.ID,
        // customerId = u.CustomerID,
        // products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
        // {
        // productId = l.ProductID,
        // amount = l.Amount
        // }).ToList(),
        // status = u.Status,
        // description = u.Description,
        // updateDate = u.UpdateDate
        // }).ToList()
        // };
        // }
        // else
        // {
        //                          var c = _context.cart.ToList();
        //                          var lenght = c.Count();
        //                          if (lenght == 0)
        //                          {
        //                              return NotFound();
        //                          }
        //                          if ((page - 1) * cartsPerPage > lenght)
        //                          {
        //                              page = (lenght / (cartsPerPage));
        //                          }
        //                          if (page * cartsPerPage > lenght && (page - 1) * cartsPerPage < lenght)
        //                          {
        //                              cartsPerPage = lenght - (page - 1) * cartsPerPage;

        //                          }
        //                          if (cartsPerPage > lenght)
        //                          {
        //                              page = 1;
        //                              cartsPerPage = lenght;
        //                          }
        //                          carts = new cartModel
        // {
        // page = page,
        // cartsPerPage = cartsPerPage,
        // carts = c
        // .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
        // {
        // id = u.ID,
        // customerId = u.CustomerID,
        // products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
        // {
        // productId = l.ProductID,
        // amount = l.Amount
        // }).ToList(),
        // status = u.Status,
        // description = u.Description,
        // updateDate = u.UpdateDate
        // }).ToList()
        // };

        // }
        // }
        // return Ok(carts);
        // }
        // else
        // {
        // return StatusCode(StatusCodes.Status403Forbidden);
        // }
        // }
        // catch
        // {
        // return StatusCode(StatusCodes.Status500InternalServerError);
        // }
        //}




        [HttpGet]
        [Route("admin/carts/{id:Guid}")]
        public ActionResult admincart(Guid id)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {
                    var cartId = _context.cart.SingleOrDefault(p => p.ID == id);

                    if (cartId == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        eachCart cart = new eachCart()
                        {
                            id = cartId.ID,
                            customerId = cartId.CustomerID,
                            products = _context.productPrices.Where(f => f.SellerID == id).Select(l => new eachproduct
                            {
                                productId = l.ProductID,
                                amount = l.Amount
                            }).ToList(),
                            status = cartId.Status,
                            description = cartId.Description,
                            updateDate = cartId.UpdateDate
                        };
                        return Ok(cart);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }





        [HttpPut]
        [Route("admin/carts/{id}")]
        public ActionResult admincartsput(Guid id, string status)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == "admin")
                {
                    var cartId = _context.cart.SingleOrDefault(p => p.ID == id);

                    if (cartId == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        cartId.Status = status;
                        _context.SaveChanges();
                        Logger.LoggerFunc(User.FindFirstValue(ClaimTypes.Name), "Put", "Create_Cart_by_ID");
                        return Ok(cartId);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpGet]
        [Route("admin/stats")]
        public ActionResult<IEnumerable<statsModel>> sellerstate(Guid sellerId, statsReqModel s, [FromQuery] int page, [FromQuery] int statsPerPage)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == "admin")
                {
                    var stats = new statsModel
                    {
                        page = page,
                        allstatsPerPage = statsPerPage,
                        stats = _context.stats.Where(d => d.productId == s.productId && d.sellerId == sellerId && d.date >= s.datefrom && d.date <= s.dateto)
                    .Skip((page - 1) * statsPerPage)
                    .Take(statsPerPage)
                    .Select(u => new statModel
                    {
                        id = u.Id,
                        productId = u.productId,
                        sellerId = u.sellerId,
                        date = u.date,
                        amount = u.amount,
                        price = u.price
                    }).ToList()

                    };
                    if (stats.stats.Count == 0)
                        return StatusCode(StatusCodes.Status400BadRequest);

                    return Ok(stats);
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}