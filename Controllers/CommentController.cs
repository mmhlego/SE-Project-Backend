using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations.Rules;
using MyOnlineShop.Data;
using MyOnlineShop.Models;

using System.Web.Http;
using System.Web.Mvc;
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
        public IActionResult getComments(Models.apimodel.CommentModel commentModel) {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                List<Comment> comments = _context.comment.Where(c => c.ProductId == commentModel.productId).ToList();

                if (comments != null)
                {
                    List<Comment> commentsForShow;
                    if ((commentModel.page * commentModel.commentsPerPage) - commentModel.commentsPerPage < comments.Count)
                    {
                        if (commentModel.page * commentModel.commentsPerPage > comments.Count)
                        {
                            commentsForShow = comments.GetRange((commentModel.page * commentModel.commentsPerPage) - commentModel.commentsPerPage, comments.Count);

                        }
                        else
                        {
                            commentsForShow = comments.GetRange((commentModel.page * commentModel.commentsPerPage) - commentModel.commentsPerPage, commentModel.page * commentModel.commentsPerPage);

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
                foreach (var comm in comments) {

                    var temp = new Models.apimodel.Comment()
                    {
                        dislikes = comm.dislikes,
                        id = comm.Id,
                        likes = comm.likes,
                        productId = comm.ProductId,
                        SendDate = comm.SentDate,
                        Text = comm.Text,
                        userImage = comm.user.ImageUrl,
                        username = comm.user.UserName
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

            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }


        [HttpGet]
        [Route("comments/{id:Guid}")]
        public IActionResult getComment(Guid id)
        {
            try
            {
                var comment = _context.comment.SingleOrDefault(c => c.Id == id);
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
                        userImage = comment.user.ImageUrl,
                        username = comment.user.UserName
                    };
                    return Ok(temp);
                }
                else {

                }
            }
            catch {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return BadRequest();
        }

        //need auth to access denied
        [HttpDelete]
        [Route("comments/{id:Guid}")]
        public IActionResult removeComment (Guid id)
        {
            var comment = _context.comment.SingleOrDefault(c => c.Id == id);
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
                    userImage = comment.user.ImageUrl,
                    username = comment.user.UserName
                };
                _context.comment.Remove(comment);
                return Ok(temp);
            }
            
            //
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
                return Ok(allComments);

            }

            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }



    }
}
