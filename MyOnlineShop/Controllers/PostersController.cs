using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyOnlineShop.Data;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;
using System.Security.Claims;
using posters = MyOnlineShop.Models.posters;

namespace MyOnlineShop.Controllers
{

    public class PostersController : Controller
    {
        private readonly MyShopContext _context;

        public PostersController(MyShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("posters/")]
        public ActionResult<IEnumerable<Pagination<posters>>> GetPosters(int page, int postersPerPage)
        {
            try
            {

                var length = _context.posters.ToList().Count();
                var totalPages = (int)Math.Ceiling((decimal)length / (decimal)postersPerPage);
                page = Math.Min(totalPages, page);
                var start = Math.Max((page - 1) * postersPerPage, 0);
                var end = Math.Min(page * postersPerPage, length);
                var count = Math.Max(end - start, 0);

                var posters = new Pagination<posters>
                {
                    page = page,
                    totalPages = totalPages,
                    perPage = postersPerPage,
                    data = _context.posters
                    .Skip(start)
                    .Take(count)
                    .Select(u => new posters
                    {
                        id = u.id,
                        title = u.title,
                        imageUrl = u.imageUrl

                    })
                    .ToList()
                };
                if (posters.data == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                return Ok(posters);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPost]
        [Route("posters/")]
        public IActionResult postPoster([FromBody] Models.apimodel.postPosters post)
        {

            try
            {

                string username = User.FindFirstValue(ClaimTypes.Name);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (username != null)
                {
                    var user = _context.users.Single(u => u.UserName == username);


                    var postersToAdd = new posters();
                    if (user.AccessLevel.ToLower() == "admin")
                    {
                        postersToAdd.id = Guid.NewGuid();
                        postersToAdd.title = post.title;
                        postersToAdd.imageUrl = post.imageUrl;
                        
                        _context.posters.Add(postersToAdd);
                        _context.SaveChanges();
                    }
                    _context.SaveChanges();

                    Models.apimodel.posters poster = new Models.apimodel.posters()
                    {
                        id = postersToAdd.id,
                        title = postersToAdd.title,
                        imageUrl = postersToAdd.imageUrl
                    };                    
                    return Ok(poster);

                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

            }

            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }


        [HttpDelete]
        [Route("posters/{id:Guid}")]
        public IActionResult removePoster(Guid id)
        {

            string username = User.FindFirstValue(ClaimTypes.Name);
            var poster1 = _context.posters.ToList();
            posters poster = null;
   
            foreach (var t in poster1)
            {
                if (t.id == id)
                {
                    poster = poster1.Single(x => x.id == id);
                }
            }


            if (username != null)
            {
                var user = _context.users.Single(u => u.UserName == username);
                string accesslevel = user.AccessLevel.ToLower();

                if (accesslevel == "admin")
                {
                    if (poster == null)
                    {                    
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        var temp = new Models.apimodel.posters()
                        {
                            id = id,
                            imageUrl = poster.imageUrl,
                            title = poster.title
                        };
                        _context.posters.Remove(poster);
                        _context.SaveChanges();
                        
                        return Ok(temp);
                    }
                }
                else
                {                    
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

            }
            else
            {              
                return StatusCode(StatusCodes.Status401Unauthorized);

            }
        }
    }
    }


