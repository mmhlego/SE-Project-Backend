using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Models.apimodel;
using MyOnlineShop.Services;
using System.Security.Claims;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using Comment = MyOnlineShop.Models.Comment;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace MyOnlineShop.Controllers
{
	public class CommentController : ControllerBase
	{
		MyShopContext _context;
		public CommentController(MyShopContext contex)
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
				List<Comment> FilteredComments = comments.ToList();

				if (commentModel.productId != default(Guid))
				{
					FilteredComments = FilteredComments.Where(c => c.ProductId == commentModel.productId).ToList();
				}
				if (commentModel.userId != default(Guid))
				{
					FilteredComments = FilteredComments.Where(c => c.UserId == commentModel.userId).ToList();
				}

				if (commentModel.dateFrom != default(DateTime))
				{
					FilteredComments = FilteredComments.Where(c => c.SentDate >= commentModel.dateFrom).ToList();
				}
				if (commentModel.dateTo != default(DateTime))
				{
					FilteredComments = FilteredComments.Where(c => c.SentDate <= commentModel.dateTo).ToList();
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

			catch (Exception e)
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
			if (userId != null)
			{
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
						_context.SaveChanges();
                        Logger.LoggerFunc(DateTime.Now, $"comments/{id:Guid}",
                            _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, temp);
                        return Ok(temp);
					}
				}
				else
				{
					return Forbid();
				}

			}
			else
			{
				return Unauthorized();
				;
			}
		}


		[Authorize]
		[HttpPost]
		[Route("comments/")]
		public IActionResult getAllComments([FromBody] Models.apimodel.postComment commentModel)
		{

			try
			{
				Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (userId != null)
				{
					var commentToAdd = new Comment();


					if (commentModel.userId == commentModel.userId)
					{

						commentToAdd.Id = Guid.NewGuid();
						commentToAdd.UserId = commentModel.userId;
						commentToAdd.ProductId = commentModel.productId;
						commentToAdd.dislikes = 0;
						commentToAdd.likes = 0;
						commentToAdd.SentDate = DateTime.Now;
						commentToAdd.Text = commentModel.text;


						_context.comment.Add(commentToAdd);
						_context.SaveChanges();
					}
					else
					{
						return Forbid();
					}
					_context.SaveChanges();

					Models.apimodel.Comment allComments = new Models.apimodel.Comment()
					{
						id = commentToAdd.Id,
						username = "test",
						userImage = "test",
						dislikes = 0,
						likes = 0,
						productId = commentToAdd.ProductId,
						SendDate = commentToAdd.SentDate,
						Text = commentToAdd.Text
					};
                    Logger.LoggerFunc(DateTime.Now, "comments",
                            _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, allComments);
                    return Ok(allComments);

				}
				else
				{
					return Unauthorized();
				}

			}

			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}


		}


		[HttpPut]
		[Route("comments/{id:Guid}/likes")]
		[Authorize]
		public IActionResult putComment(Guid id, [FromBody] likeModel l)
		{
			try
			{
				var comment = _context.comment.Where(p => p.Id == id).Single();
				if (comment == null)
				{
					return NotFound();
				}

				else
				{

					if (l.like == true)
					{
						comment.likes = comment.likes + 1;

					}
					else
					{
						comment.dislikes = comment.dislikes + 1;

					}
					_context.Update(comment);
					_context.SaveChanges();
					if (!ModelState.IsValid)
					{
						return BadRequest(ModelState);
					}
                    Logger.LoggerFunc(DateTime.Now, $"comments/{id:Guid}/likes",
                            _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, Ok());
                    return Ok();
				}
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
