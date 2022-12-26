using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.OpenApi.Validations.Rules;
using MyOnlineShop.Data;
using MyOnlineShop.Models;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Mvc;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace MyOnlineShop.Controllers
{
    public class CommentController : ControllerBase
    {
        MyShopContex _context;
        public CommentController(MyShopContex contex)
        {
            _context = contex;
        }
        [HttpGet]
        [Route("comments/")]
        public IActionResult getComments(Models.apimodel.CommentModel commentModel)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                List<Comment> comments = _context.comment.ToList();
                List<Comment> FilteredComments;
                if (commentModel.productId != null && commentModel.userId == null && commentModel.dateFrom!=null
                    && commentModel.dateTo == null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.ProductId == commentModel.productId 
                    && c.SentDate>commentModel.dateFrom);
                }
                else if (commentModel.productId != null && commentModel.userId != null 
                    && commentModel.dateFrom != null && commentModel.dateTo == null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.ProductId == commentModel.productId
                    && c.UserId == commentModel.userId && c.SentDate > commentModel.dateFrom);
                }
                else if (commentModel.productId == null && commentModel.userId != null 
                    && commentModel.dateFrom != null && commentModel.dateTo == null) {
                    FilteredComments = (List<Comment>)comments.Where(c => c.UserId == commentModel.userId
                    && c.SentDate > commentModel.dateFrom);
                }
                else if (commentModel.productId != null && commentModel.userId == null &&
                    commentModel.dateFrom == null && commentModel.dateTo == null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.ProductId == commentModel.productId);
                }
                else if (commentModel.productId != null && commentModel.userId != null 
                    && commentModel.dateFrom == null && commentModel.dateTo == null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.ProductId == commentModel.productId
                    && c.UserId == commentModel.userId);
                }
                else if (commentModel.productId == null && commentModel.userId != null 
                    && commentModel.dateFrom == null && commentModel.dateTo == null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.UserId == commentModel.userId);
                }
                else if (commentModel.productId != null && commentModel.userId == null && commentModel.dateFrom != null
                    && commentModel.dateTo != null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.ProductId == commentModel.productId
                    && c.SentDate > commentModel.dateFrom && c.SentDate < commentModel.dateTo);
                }
                else if (commentModel.productId != null && commentModel.userId != null
                    && commentModel.dateFrom != null && commentModel.dateTo != null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.ProductId == commentModel.productId
                    && c.UserId == commentModel.userId && c.SentDate > commentModel.dateFrom && c.SentDate < commentModel.dateTo);
                }
                else if (commentModel.productId == null && commentModel.userId != null
                    && commentModel.dateFrom != null && commentModel.dateTo != null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.UserId == commentModel.userId
                    && c.SentDate > commentModel.dateFrom && c.SentDate < commentModel.dateTo);
                }
                else if (commentModel.productId != null && commentModel.userId == null &&
                    commentModel.dateFrom == null && commentModel.dateTo != null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.ProductId == commentModel.productId 
                    && c.SentDate < commentModel.dateTo);
                }
                else if (commentModel.productId != null && commentModel.userId != null
                    && commentModel.dateFrom == null && commentModel.dateTo != null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.ProductId == commentModel.productId
                    && c.UserId == commentModel.userId && c.SentDate < commentModel.dateTo);
                }
                else if (commentModel.productId == null && commentModel.userId != null
                    && commentModel.dateFrom == null && commentModel.dateTo != null)
                {
                    FilteredComments = (List<Comment>)comments.Where(c => c.UserId == commentModel.userId 
                    && c.SentDate <commentModel.dateTo);
                }
                else
                {
                    FilteredComments = comments;
                }


                List<Comment> commentsForShow;
              
                if (FilteredComments != null)
                {
                    
                    if ((commentModel.page * commentModel.commentsPerPage) - commentModel.commentsPerPage < FilteredComments.Count)
                    {
                        if (commentModel.page * commentModel.commentsPerPage > FilteredComments.Count)
                        {
                            commentsForShow = FilteredComments.GetRange((commentModel.page * commentModel.commentsPerPage) - commentModel.commentsPerPage, FilteredComments.Count);

                        }
                        else
                        {
                            commentsForShow = FilteredComments.GetRange((commentModel.page * commentModel.commentsPerPage) - commentModel.commentsPerPage, commentModel.page * commentModel.commentsPerPage);

                        }
                       
                    }
                    else
                    {
                        return BadRequest();
                    }
                }


                else
                {
                    return NotFound();

                }

                List<Models.apimodel.Comment> commentsSchema = new List<Models.apimodel.Comment>();
                foreach (var comm in commentsForShow)
                {
                    var user = _context.users.Single(u => u.ID == comm.UserId);
                    var temp = new Models.apimodel.Comment()
                    {
                        dislikes = comm.dislikes,
                        id = comm.Id,
                        likes = comm.likes,
                        productId = comm.ProductId,
                        SendDate = comm.SentDate,
                        Text = comm.Text,
                        userImage = user.ImageUrl,
                        username = user.UserName
                    };
                    commentsSchema.Add(temp);
                }


                Models.apimodel.Comments allComments = new Models.apimodel.Comments()
                {
                    page = commentModel.page,
                    commentsPerPage = commentModel.commentsPerPage,
                    comments = commentsSchema
                };
                return Ok(allComments);

            }

            catch(Exception e)
            {
                return Ok(e);
            }


        }


        [HttpGet]
        [Route("comments/{id:Guid}")]
        public IActionResult getComment(Guid id)
        {
            try
            {
                var comment = _context.comment.SingleOrDefault(c => c.Id == id);
                var user = _context.users.Single(u => u.ID == comment.UserId);
                if (comment != null)
                {
                    var temp = new Models.apimodel.Comment()
                    {
                        dislikes = comment.dislikes,
                        id = comment.Id,
                        likes = comment.likes,
                        productId = comment.ProductId,
                        SendDate = comment.SentDate,
                        Text = comment.Text,
                        userImage = user.ImageUrl,
                        username = user.UserName
                    };
                    return Ok(temp);
                }
                else
                {
                    
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return BadRequest();
        }

        //need auth to access denied
        [HttpDelete]
        [Route("comments/{id:Guid}")]
        [Authorize]
        public IActionResult removeComment(Guid id)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            var comment = _context.comment.SingleOrDefault(c => c.Id == id);
            var user = _context.users.Single(u => u.ID == userId);
            string accesslevel = user.AccessLevel.ToLower();
            if (accesslevel == "admin")
            {
                if (comment == null)
                {

                    return NotFound();
                }
                else
                {
                    var temp = new Models.apimodel.Comment()
                    {
                        dislikes = comment.dislikes,
                        id = comment.Id,
                        likes = comment.likes,
                        productId = comment.ProductId,
                        SendDate = comment.SentDate,
                        Text = comment.Text,
                        userImage = user.ImageUrl,
                        username = user.UserName
                    };
                    _context.comment.Remove(comment);
                    return Ok(temp);
                }
            }
            else
            {
                return Forbid();
            }

        }


        [HttpPost]
        [Route("comments/")]
        public IActionResult getAllComments([FromBody] Models.apimodel.CommentModel commentModel)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Models.apimodel.Comments allComments = new Models.apimodel.Comments();


                _context.SaveChanges();
                return Ok(allComments);

            }

            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }



    }
}
