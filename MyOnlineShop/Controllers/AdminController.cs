using System;
using System.Diagnostics;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;
using Newtonsoft.Json;
using NuGet.Packaging.Licenses;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using MyOnlineShop.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Control;


namespace MyOnlineShop.Controllers
{

    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly MyShopContex _context;

        public AdminController(MyShopContex context)
        {
            _context = context;
        }

        //------------------------------------
        [HttpGet]
        [Route("admin/users")]
        public ActionResult<IEnumerable<usersModel>> userssget([FromQuery] int page, [FromQuery] int usersPerPage)
        {

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
                        restricted = u.Restricted
                    })
                    .ToList()
            };

            return Ok(users);
        }

        //------------------------------------

        [HttpGet]
        [Route("admin/users/{id}")]
        public ActionResult eachuserget(Guid id)
        {
            if (User.IsInRole("Administrator"))
            {
                try
                {
                    string finderQry = ("select * from User where id = " + id);
                    SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                    //userModel s = new userModel();
                    SqlConnection connect = new SqlConnection(conn.ConnectionString);
                    connect.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand samplecommand = new SqlCommand(finderQry, connect);
                    da.SelectCommand = samplecommand;
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if (ds == null)
                        return StatusCode(StatusCodes.Status404NotFound);
                    else
                    {
                        var user_in_json_id = JsonConvert.SerializeObject(ds);

                        if (!ModelState.IsValid)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest);
                        }
                        return Content(user_in_json_id);
                    }
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);


        }



        [HttpPut]
        [Route("admin/users/{id}")]
        public ActionResult userput(Guid id, [FromBody] userreqModel req)
        {
            if (User.IsInRole("Administrator"))
            {
                try
                {
                    string updateQry = ("update User set phoneNumber = " + req.phoneNumber + ", email = " + req.email + ", accessLevel = " + req.accessLevel + ", restricted = " + req.restricted + " where id = " + id);
                    SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                    //userModel s = new userModel();
                    SqlConnection connect = new SqlConnection(conn.ConnectionString);
                    connect.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand samplecommand = new SqlCommand(updateQry, connect);
                    da.SelectCommand = samplecommand;
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    var user_in_json_id_up = JsonConvert.SerializeObject(ds);

                    if (!ModelState.IsValid)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                    return Content(user_in_json_id_up);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);



        }




        [HttpDelete]
        [Route("admin/users/{id}")]
        public ActionResult userdelete(Guid id)
        {
            if (User.IsInRole("Administrator"))
            {
                try
                {
                    DataSet ds = new DataSet();
                    string deleteQry = ("update User set restrict = true where id = " + id);
                    SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                    //userModel s = new userModel();
                    SqlConnection connect = new SqlConnection(conn.ConnectionString);
                    connect.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand samplecommand = new SqlCommand(deleteQry, connect);
                    da.SelectCommand = samplecommand;
                    da.Fill(ds);
                    if (ds == null)
                        return StatusCode(StatusCodes.Status404NotFound);
                    else
                    {
                        var user_in_json_id = JsonConvert.SerializeObject(ds);

                        if (!ModelState.IsValid)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest);
                        }
                        return Content(user_in_json_id);
                    }
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);



        }



        [HttpGet]
        [Route("admin/discountTokens")]
        public ActionResult<IEnumerable<tokensModel>> discountTokens(bool isEvent, bool expired, [FromQuery] int page, [FromQuery] int tokensPerPage)
        {
            if (User.IsInRole("Administrator"))
            {
                try
                {
                    var tokens = new tokensModel();
                    if (expired)
                    {
                        tokens = new tokensModel
                        {
                            page = page,
                            tokensPerPage = tokensPerPage,
                            tokens = _context.giftCards.Where(d => d.ExpirationDate < DateTime.Now && d.IsEvent == isEvent)
                                                    .Skip((page - 1) * tokensPerPage)
                                                    .Take(tokensPerPage)
                                                    .Select(u => new GiftCard
                                                    {
                                                        Id = u.Id,
                                                        ExpirationDate = u.ExpirationDate,
                                                        Discount = u.Discount,
                                                        IsEvent = u.IsEvent
                                                    }).ToList()

                        };
                        if (tokens.tokens.Count == 0)
                            return StatusCode(StatusCodes.Status400BadRequest);

                        return Ok(tokens);
                    }
                    else
                    {
                        tokens = new tokensModel
                        {
                            page = page,
                            tokensPerPage = tokensPerPage,
                            tokens = _context.giftCards.Where(d => d.ExpirationDate >= DateTime.Now && d.IsEvent == isEvent)
                                                    .Skip((page - 1) * tokensPerPage)
                                                    .Take(tokensPerPage)
                                                    .Select(u => new GiftCard
                                                    {
                                                        Id = u.Id,
                                                        ExpirationDate = u.ExpirationDate,
                                                        Discount = u.Discount,
                                                        IsEvent = u.IsEvent
                                                    }).ToList()

                        };
                        if (tokens.tokens.Count == 0)
                            return StatusCode(StatusCodes.Status400BadRequest);

                        return Ok(tokens);
                    }
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);
        }



        [HttpPost]
        [Route("admin/discountTokens")]
        public ActionResult discountTokenspost([FromBody] tokenreqModel tokenreq)
        {
            if (User.IsInRole("Administrator"))
            {
                try
                {
                    DataSet ds = new DataSet();
                    string insertQry = ("insert into GiftCard (ExpirationDate, Discount, IsEvent) values (" + tokenreq.ExpirationDate + ", " + tokenreq.Discount + ", " + tokenreq.IsEvent + ")");
                    SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();

                    SqlConnection connect = new SqlConnection(conn.ConnectionString);
                    connect.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand samplecommand = new SqlCommand(insertQry, connect);
                    da.SelectCommand = samplecommand;
                    da.Fill(ds);
                    if (ds == null)
                        return StatusCode(StatusCodes.Status404NotFound);
                    else
                    {
                        var insert_token_in_json = JsonConvert.SerializeObject(ds);

                        if (!ModelState.IsValid)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest);
                        }
                        return Content(insert_token_in_json);
                    }
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);



        }




        [HttpDelete]
        [Route("admin/discountTokens/{id}")]
        public ActionResult discountTokensdelete(Guid id)
        {
            if (User.IsInRole("Administrator"))
            {
                try
                {
                    DataSet ds = new DataSet();
                    string deleteQry = ("delete * from GiftCard where id = " + id);
                    SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();

                    SqlConnection connect = new SqlConnection(conn.ConnectionString);
                    connect.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand samplecommand = new SqlCommand(deleteQry, connect);
                    da.SelectCommand = samplecommand;
                    da.Fill(ds);
                    if (ds == null)
                        return StatusCode(StatusCodes.Status404NotFound);
                    else
                    {
                        var delete_token_in_json = JsonConvert.SerializeObject(ds);

                        if (!ModelState.IsValid)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest);
                        }
                        return Content(delete_token_in_json);
                    }
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);



        }

        [HttpGet]
        [Route("admin/carts")]
        public ActionResult<IEnumerable<cartModel>> admincarts(Guid? userId, bool? current, [FromQuery] int page, [FromQuery] int cartsPerPage)
        {
            try
            {
                var carts = new cartModel();
                if (current.HasValue)
                {
                    if (current.Value == true)
                    {
                        //current = True => status = Approved
                        if (userId.HasValue)
                        {
                            carts = new cartModel
                            {
                                page = page,
                                cartsPerPage = cartsPerPage,
                                carts = _context.cart.Where(d => d.Status == "Approved" && d.ID == userId)
                                .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
                                {
                                    id = u.ID,
                                    customerId = u.CustomerID,
                                    products = _context.productPrices.Select(l => new eachproduct
                                    {
                                        productId = l.ProductID,
                                        amount = l.Amount
                                    }).ToList(),
                                    status = u.Status,
                                    description = u.Discription,
                                    updateDate = u.UpdateDate
                                }).ToList()
                            };
                        }
                        else
                        {
                            carts = new cartModel
                            {
                                page = page,
                                cartsPerPage = cartsPerPage,
                                carts = _context.cart.Where(d => d.Status == "Approved")
                                .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
                                {
                                    id = u.ID,
                                    customerId = u.CustomerID,
                                    products = _context.productPrices.Select(l => new eachproduct
                                    {
                                        productId = l.ProductID,
                                        amount = l.Amount
                                    }).ToList(),
                                    status = u.Status,
                                    description = u.Discription,
                                    updateDate = u.UpdateDate
                                }).ToList()
                            };

                        }
                    }
                    else if (current.Value == false)
                    {
                        //current = fasle => status = Rejected
                        if (userId.HasValue)
                        {
                            carts = new cartModel
                            {
                                page = page,
                                cartsPerPage = cartsPerPage,
                                carts = _context.cart.Where(d => d.Status == "Rejected" && d.ID == userId)
                                .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
                                {
                                    id = u.ID,
                                    customerId = u.CustomerID,
                                    products = _context.productPrices.Select(l => new eachproduct
                                    {
                                        productId = l.ProductID,
                                        amount = l.Amount
                                    }).ToList(),
                                    status = u.Status,
                                    description = u.Discription,
                                    updateDate = u.UpdateDate
                                }).ToList()
                            };
                        }
                        else
                        {
                            carts = new cartModel
                            {
                                page = page,
                                cartsPerPage = cartsPerPage,
                                carts = _context.cart.Where(d => d.Status == "Rejected")
                                .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
                                {
                                    id = u.ID,
                                    customerId = u.CustomerID,
                                    products = _context.productPrices.Select(l => new eachproduct
                                    {
                                        productId = l.ProductID,
                                        amount = l.Amount
                                    }).ToList(),
                                    status = u.Status,
                                    description = u.Discription,
                                    updateDate = u.UpdateDate
                                }).ToList()
                            };

                        }
                    }
                }
                else
                {
                    if (userId.HasValue)
                    {
                        carts = new cartModel
                        {
                            page = page,
                            cartsPerPage = cartsPerPage,
                            carts = _context.cart.Where(d => d.ID == userId)
                            .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
                            {
                                id = u.ID,
                                customerId = u.CustomerID,
                                products = _context.productPrices.Select(l => new eachproduct
                                {
                                    productId = l.ProductID,
                                    amount = l.Amount
                                }).ToList(),
                                status = u.Status,
                                description = u.Discription,
                                updateDate = u.UpdateDate
                            }).ToList()
                        };
                    }
                    else
                    {
                        carts = new cartModel
                        {
                            page = page,
                            cartsPerPage = cartsPerPage,
                            carts = _context.cart
                            .Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
                            {
                                id = u.ID,
                                customerId = u.CustomerID,
                                products = _context.productPrices.Select(l => new eachproduct
                                {
                                    productId = l.ProductID,
                                    amount = l.Amount
                                }).ToList(),
                                status = u.Status,
                                description = u.Discription,
                                updateDate = u.UpdateDate
                            }).ToList()
                        };

                    }
                }
                return Ok(carts);

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
            if (User.IsInRole("Administrator"))
            {
                try
                {
                    string finderQry = ("select * from Cart where id = " + id);
                    SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                    SqlConnection connect = new SqlConnection(conn.ConnectionString);
                    connect.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand samplecommand = new SqlCommand(finderQry, connect);
                    da.SelectCommand = samplecommand;
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if (ds == null)
                        return StatusCode(StatusCodes.Status404NotFound);
                    else
                    {
                        var cart_in_json_id = JsonConvert.SerializeObject(ds);

                        if (!ModelState.IsValid)
                        {
                            return StatusCode(StatusCodes.Status400BadRequest);
                        }
                        return Content(cart_in_json_id);
                    }
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);
        }





        [HttpPut]
        [Route("admin/carts/{id}")]
        public ActionResult admincartsput(Guid id, string status)
        {
            if (User.IsInRole("Administrator"))
            {
                try
                {
                    string updateQry = ("update Cart set status = " + status + " where id = " + id);
                    SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
                    SqlConnection connect = new SqlConnection(conn.ConnectionString);
                    connect.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand samplecommand = new SqlCommand(updateQry, connect);
                    da.SelectCommand = samplecommand;
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    var cart_in_json_id_up = JsonConvert.SerializeObject(ds);

                    if (!ModelState.IsValid)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                    return Content(cart_in_json_id_up);
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);

        }



        [HttpGet]
        [Route("admin/stats")]
        public ActionResult<IEnumerable<statsModel>> sellerstate(Guid sellerId, statsReqModel s, [FromQuery] int page, [FromQuery] int statsPerPage)
        {
            if (User.IsInRole("Administrator"))
            {
                try
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
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
                return StatusCode(StatusCodes.Status403Forbidden);



        }


    }
}



