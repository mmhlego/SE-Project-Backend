using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;
using System.Data;
using MyOnlineShop.Data;
using MyOnlineShop.Services;
using System.Security.Claims;

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
        public ActionResult<IEnumerable<Pagination<userModel>>> userssget([FromQuery] int page, [FromQuery] int usersPerPage)
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
                    var length = _context.users.ToList().Count();
                    var totalPages = (int)Math.Ceiling((decimal)length / (decimal)usersPerPage);
                    page = Math.Min(totalPages, page);
                    var start = Math.Max((page - 1) * usersPerPage, 0);
                    var end = Math.Min(page * usersPerPage, length);
                    var count = Math.Max(end - start, 0);
                    usersPerPage = count;

                    var users = new Pagination<userModel>
                    {
                        page = page,
                        totalPages = totalPages,
                        perPage = usersPerPage,
                        data = _context.users
                    .Skip(start)
                    .Take(count)
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
                    if (users.data == null)
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
                    Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        req, Unauthorized());
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {
                    var userId = _context.users.SingleOrDefault(p => p.ID == id);

                    if (userId == null)
                    {
                        Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        req, StatusCode(StatusCodes.Status404NotFound));
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
                        Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            req, u);
                        return Ok(u);
                    }

                    if (!ModelState.IsValid)
                    {
                        Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            req, StatusCode(StatusCodes.Status400BadRequest));
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            req, StatusCode(StatusCodes.Status403Forbidden));
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            req, StatusCode(StatusCodes.Status500InternalServerError));
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
                    Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        id, Unauthorized());
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {

                    var userId = _context.users.SingleOrDefault(p => p.ID == id);
                    bool restricted = true;
                    if (userId == null)
                    {
                        Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        id, StatusCode(StatusCodes.Status404NotFound));
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
                        Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            id, u);
                        return Ok(u);
                    }
                }
                else
                {
                    Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            id, StatusCode(StatusCodes.Status403Forbidden));
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                Logger.LoggerFunc($"admin/users/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            id, StatusCode(StatusCodes.Status500InternalServerError));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpGet]
        [Route("admin/discountTokens")]
        public ActionResult<IEnumerable<Pagination<DiscountToken>>> discountTokens(bool isEvent, bool expired, int page, int tokensPerPage)
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

                    var length = _context.tokens.ToList().Count();
                    if (length == 0)
                    {
                        return NotFound();
                    }
                    var totalPages = (int)Math.Ceiling((decimal)length / (decimal)tokensPerPage);
                    page = Math.Min(totalPages, page);
                    var start = Math.Max((page - 1) * tokensPerPage, 0);
                    var end = Math.Min(page * tokensPerPage, length);
                    var count = Math.Max(end - start, 0);
                    tokensPerPage = count;
                    var tokens = new Pagination<DiscountToken>
                    {
                        page = page,
                        perPage = tokensPerPage,
                        totalPages = totalPages,
                        data = alltokens
                        .Skip(start)
                        .Take(count)
                        .Select(u => new DiscountToken
                        {
                            Id = u.Id,
                            ExpirationDate = u.ExpirationDate,
                            Discount = u.Discount,
                            IsEvent = u.IsEvent
                        }).ToList()

                    };
                    if (tokens.data.Count == 0)
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
        public ActionResult discountTokenspost([FromBody] tokenresmodel tokenputter)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    Logger.LoggerFunc("admin/discountTokens", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        tokenputter, Unauthorized());
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
                    tokenresmodel t = new tokenresmodel()
                    {
                        discount = tokenput.Discount,
                        id = tokenput.Id,
                        isEvent = tokenput.IsEvent,
                        expirationDate = tokenput.ExpirationDate
                    };
                    Logger.LoggerFunc("admin/discountTokens", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        tokenputter, t);
                    return Ok(t);

                    if (!ModelState.IsValid)
                    {
                        Logger.LoggerFunc("admin/discountTokens", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            tokenputter, StatusCode(StatusCodes.Status400BadRequest));
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    Logger.LoggerFunc("admin/discountTokens", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        tokenputter, StatusCode(StatusCodes.Status403Forbidden));
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                Logger.LoggerFunc("admin/discountTokens", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        tokenputter, StatusCode(StatusCodes.Status500InternalServerError));
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
                    Logger.LoggerFunc($"admin/discountTokens/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        id, Unauthorized());
                    return Unauthorized();
                }
                if (accessLevel == "admin")
                {
                    var delToken = _context.tokens.SingleOrDefault(p => p.Id == id);
                    if (delToken == null)
                    {
                        Logger.LoggerFunc($"admin/discountTokens/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            id, StatusCode(StatusCodes.Status404NotFound));
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
                        Logger.LoggerFunc($"admin/discountTokens/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            id, t);
                        return Ok(t);

                    }



                }
                else
                {
                    Logger.LoggerFunc($"admin/discountTokens/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        id, StatusCode(StatusCodes.Status403Forbidden));
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
            catch
            {
                Logger.LoggerFunc($"admin/discountTokens/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                        id, StatusCode(StatusCodes.Status500InternalServerError));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("admin/carts")]
        public ActionResult<IEnumerable<Pagination<eachCart>>> admincarts(Guid userId, bool current, [FromQuery] int page, [FromQuery] int cartsPerPage)
        {
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
                if (accessLevel == null)
                {
                    return Unauthorized();
                }
                if (accessLevel != "admin")
                {
                    return Forbid();
                }
                var c = _context.cart.ToList();
                //if (current.HasValue)
                //{ 
                if (current == true)
                {
                    c = c.Where(c => c.Status == "Filling").ToList();
                }
                else
                {
                    c = c.Where(c => c.Status != "Filling").ToList();
                }

                //}
                if (userId != default(Guid))
                {

                    var user = _context.customer.SingleOrDefault(c => c.UserId == userId);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    c = c.Where(c => c.CustomerID == user.ID).ToList();
                }

                var carts = new List<eachCart>();
                foreach (var cart in c)
                {

                    var eachproducts = new List<eachproduct>();
                    var orders = _context.orders.Where(o => o.CartID == cart.ID).ToList();
                    foreach (var o in orders)
                    {
                        var p = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID);

                        eachproduct eachproduct = new eachproduct()
                        {
                            productId = p.ProductID,
                            amount = o.Amount

                        };
                        eachproducts.Add(eachproduct);

                    }
                    var eachCart = new eachCart()
                    {
                        status = cart.Status,
                        customerId = cart.CustomerID,
                        description = cart.Description,
                        id = cart.ID,
                        updateDate = cart.UpdateDate,
                        products = eachproducts

                    };
                    carts.Add(eachCart);
                }


                var lenght = c.Count();
                if (lenght == 0)
                {
                    return NotFound();
                }
                var length = _context.users.ToList().Count();
                var totalPages = (int)Math.Ceiling((decimal)length / (decimal)cartsPerPage);
                page = Math.Min(totalPages, page);
                var start = Math.Max((page - 1) * cartsPerPage, 0);
                var end = Math.Min(page * cartsPerPage, length);
                var count = Math.Max(end - start, 0);
                cartsPerPage = count;
                var allcarts = new Pagination<eachCart>
                {
                    page = page,
                    totalPages = totalPages,
                    perPage = cartsPerPage,
                    data = carts
                    .Skip(start).Take(count).ToList()
                };
                return Ok(allcarts);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




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
                        var eachproducts = new List<eachproduct>();
                        var orders = _context.orders.Where(o => o.CartID == id).ToList();
                        foreach (var o in orders)
                        {
                            var p = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID);

                            eachproduct eachproduct = new eachproduct()
                            {
                                productId = p.ProductID,
                                amount = o.Amount

                            };
                            eachproducts.Add(eachproduct);

                        }
                        eachCart cart = new eachCart()
                        {
                            id = cartId.ID,
                            customerId = cartId.CustomerID,
                            products = eachproducts,
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
                        if (cartId.Status.ToLower() == "pending")
                        {
                            if (status.ToLower() == "rejected")
                            {
                                var orders = _context.orders.Where(o => o.CartID == id).ToList();
                                foreach (var o in orders)
                                {
                                    var p = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID);

                                    p.Amount = o.Amount + p.Amount;
                                    o.Amount = 0;

                                    _context.Update(p);
                                    _context.Update(o);
                                    _context.SaveChanges();



                                }
                                cartId.Status = "Rejected";
                                _context.Update(cartId);
                                _context.SaveChanges();

                            }
                            if (status.ToLower() == "approved")
                            {
                                cartId.Status = "Approved";
                                _context.Update(cartId);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            return BadRequest();
                        }

                        cartId.Status = status;

                        _context.Update(cartId);
                        _context.SaveChanges();
                        var s = new Dictionary<string, string>() { { "status", cartId.Status } };
                        //Logger.LoggerFunc(User.FindFirstValue(ClaimTypes.Name), "Put", "Create_Cart_by_ID");
                        return Ok(s);
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
        public ActionResult<IEnumerable<Pagination<statModel>>> sellerstate(Guid sellerId, statsReqModel s, [FromQuery] int page, [FromQuery] int statsPerPage)
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
                    var s1 = _context.stats.ToList();
                    if (sellerId != default(Guid))
                    {
                        s1 = s1.Where(p => p.sellerId == sellerId).ToList();
                    }
                    if (s.productId != default(Guid))
                    {
                        s1 = s1.Where(p => p.productId == s.productId).ToList();
                    }
                    if (s.datefrom != default(DateTime))
                    {
                        s1 = s1.Where(p => p.date >= s.datefrom).ToList();
                    }
                    if (s.dateto != default(DateTime))
                    {
                        s1 = s1.Where(p => p.date <= s.dateto).ToList();
                    }
                    var length = s1.Count();

                    if (length == 0)
                    {
                        return NotFound();
                    }

                    var totalPages = (int)Math.Ceiling((decimal)length / (decimal)statsPerPage);
                    page = Math.Min(totalPages, page);
                    var start = Math.Max((page - 1) * statsPerPage, 0);
                    var end = Math.Min(page * statsPerPage, length);
                    var count = Math.Max(end - start, 0);
                    statsPerPage = count;
                    var stats = new Pagination<statModel>
                    {

                        page = page,
                        totalPages = totalPages,
                        perPage = statsPerPage,
                        data = s1
                        .Skip(start)
                        .Take(count)
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



