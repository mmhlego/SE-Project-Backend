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


				if (FilteredComments == null)
				{
					return NotFound();
				}

				var length = FilteredComments.Count();
				var totalPages = (int)Math.Ceiling((decimal)length / (decimal)commentModel.commentsPerPage);
				commentModel.page = Math.Min(totalPages, commentModel.page);
				var start = Math.Max((commentModel.page - 1) * commentModel.commentsPerPage, 0);
				var end = Math.Min(commentModel.page * commentModel.commentsPerPage, length);
				var count = Math.Max(end - start, 0);
				commentModel.commentsPerPage = count;
				commentsForShow = FilteredComments.GetRange(start, count);


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


				var allComments = new Pagination<Models.apimodel.Comment>()
				{
					page = commentModel.page,
					totalPages = totalPages,
					perPage = commentModel.commentsPerPage,
					data = commentsSchema
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
				var comment1 = _context.comment.ToList();
				Comment comment = null;
				int i = 0;
				foreach (var t in comment1)
				{
					if (t.Id == id)
					{
						comment = comment1.Single(x => x.Id == id);
						i++;
					}
				}
				if (comment != null)
				{
					var user = _context.users.Single(u => u.ID == comment.UserId);
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
					return StatusCode(StatusCodes.Status404NotFound);
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
            Logger logger = new Logger(_context);

            //Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            string username = User.FindFirstValue(ClaimTypes.Name);
			var comment1 = _context.comment.ToList();
			Comment comment = null;
			int i = 0;
			foreach (var t in comment1)
			{
				if (t.Id == id)
				{
					comment = comment1.Single(x => x.Id == id);
					i++;
				}
			}


			if (username != null)
			{
				var user = _context.users.Single(u => u.UserName == username);
				string accesslevel = user.AccessLevel.ToLower();

				if (accesslevel == "admin")
				{
					if (comment == null)
					{
						logger.LoggerFunc($"comments/{id:Guid}", 
							id, StatusCode(StatusCodes.Status404NotFound), User);
						return StatusCode(StatusCodes.Status404NotFound);
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
						logger.LoggerFunc($"comments/{id:Guid}", 
							id, temp, User);
						return Ok(temp);
					}
				}
				else
				{
					logger.LoggerFunc($"comments/{id:Guid}", 
							id, StatusCode(StatusCodes.Status403Forbidden), User);
					return StatusCode(StatusCodes.Status403Forbidden);
				}

			}
			else
			{
				logger.LoggerFunc($"comments/{id:Guid}", 
							id, StatusCode(StatusCodes.Status401Unauthorized), User);
				return StatusCode(StatusCodes.Status401Unauthorized);

			}
		}


		[Authorize]
		[HttpPost]
		[Route("comments/")]
		public IActionResult getAllComments([FromBody] Models.apimodel.postComment commentModel)
		{
            Logger logger = new Logger(_context);

            try
            {

				//Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
				string username = User.FindFirstValue(ClaimTypes.Name);
				if (!ModelState.IsValid)
				{
					logger.LoggerFunc("comments", 
							commentModel, BadRequest(ModelState), User);
					return BadRequest(ModelState);
				}
				if (username != null)
				{
					var user = _context.users.Single(u => u.UserName == username);
					var commentToAdd = new Comment();


					if (user.AccessLevel.ToLower() == "customer")
					{

						commentToAdd.Id = Guid.NewGuid();
						commentToAdd.UserId = user.ID;
						commentToAdd.ProductId = commentModel.productId;
						commentToAdd.dislikes = 0;
						commentToAdd.likes = 0;
						commentToAdd.SentDate = DateTime.Now;
						commentToAdd.Text = commentModel.text;


						_context.comment.Add(commentToAdd);
						_context.SaveChanges();
					}
					_context.SaveChanges();

					Models.apimodel.Comment allComments = new Models.apimodel.Comment()
					{
						id = commentToAdd.Id,
						username = username,
						userImage = user.ImageUrl,
						dislikes = 0,
						likes = 0,
						productId = commentToAdd.ProductId,
						SendDate = commentToAdd.SentDate,
						Text = commentToAdd.Text
					};
					logger.LoggerFunc("comments", 
							commentModel, allComments, User);
					return Ok(allComments);

				}
				else
				{
					logger.LoggerFunc("comments", 
							commentModel, StatusCode(StatusCodes.Status401Unauthorized), User);
					return StatusCode(StatusCodes.Status401Unauthorized);
				}

			}

			catch
			{
				logger.LoggerFunc("comments", 
							commentModel, StatusCode(StatusCodes.Status500InternalServerError), User);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}


		}


		[HttpPut]
		[Route("comments/{id:Guid}/likes")]
		[Authorize]
		public IActionResult putComment(Guid id, [FromBody] likeModel l)
		{
            Logger logger = new Logger(_context);

            try
            {
				if (!ModelState.IsValid)
				{
					logger.LoggerFunc($"comments/{id:Guid}/likes", 
							l, BadRequest(ModelState), User);
					return BadRequest(ModelState);
				}
				string username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.Single(u => u.UserName == username);
				if (username != null)
				{
					if (user.AccessLevel.ToLower() == "customer")
					{
						var comment1 = _context.comment.ToList();
						Comment comment = null;
						int i = 0;
						foreach (var t in comment1)
						{
							if (t.Id == id)
							{
								comment = comment1.Single(x => x.Id == id);
								i++;
							}
						}

						if (comment == null)
						{
							logger.LoggerFunc($"comments/{id:Guid}/likes", 
								l, StatusCode(StatusCodes.Status404NotFound), User);
							return StatusCode(StatusCodes.Status404NotFound);
						}

						else
						{

							if (l.like)
							{
								comment.likes = comment.likes + 1;

							}
							else
							{
								comment.dislikes = comment.dislikes + 1;

							}
							_context.Update(comment);
							_context.SaveChanges();

							Models.apimodel.Comment CommentForShow = new Models.apimodel.Comment()
							{
								id = comment.Id,
								username = username,
								userImage = user.ImageUrl,
								dislikes = comment.dislikes,
								likes = comment.likes,
								productId = comment.ProductId,
								SendDate = comment.SentDate,
								Text = comment.Text
							};

							logger.LoggerFunc($"comments/{id:Guid}/likes", 
								l, CommentForShow, User);
							return Ok(CommentForShow);
						}
					}
					else
					{
                        logger.LoggerFunc($"comments/{id:Guid}/likes",
                                l, StatusCode(StatusCodes.Status403Forbidden), User);
                        return StatusCode(StatusCodes.Status403Forbidden);
					}
				}

				else
				{
                    logger.LoggerFunc($"comments/{id:Guid}/likes",
                                l, StatusCode(StatusCodes.Status401Unauthorized), User);
                    return StatusCode(StatusCodes.Status401Unauthorized);
				}
			}
			catch
			{
                logger.LoggerFunc($"comments/{id:Guid}/likes",
                                l, StatusCode(StatusCodes.Status500InternalServerError), User);
                return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
