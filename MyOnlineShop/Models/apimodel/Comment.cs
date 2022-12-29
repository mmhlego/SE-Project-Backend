namespace MyOnlineShop.Models.apimodel
{
	public class Comments
	{
		public int commentsPerPage { get; set; }
		public int page { get; set; }

		public List<apimodel.Comment> comments { get; set; }
	}
	public class CommentModel
	{
		public CommentModel()
		{
			this.commentsPerPage = 50;
			this.page = 1;


		}

		public int commentsPerPage { get; set; }

		public int page { get; set; }
		public Guid productId { get; set; }
		public Guid userId { get; set; }
		public DateTime dateFrom { get; set; }
		public DateTime dateTo { get; set; }

	}
	public class postComment
	{
		public Guid userId { get; set; }
		public Guid productId { get; set; }

		public string text { get; set; }

	}
	public class Comment
	{
		public Guid id { get; set; }

		public string username { get; set; }

		public string userImage { get; set; }

		public Guid productId { get; set; }

		public DateTime SendDate { get; set; }

		public string Text { get; set; }
		public int likes { get; set; }
		public int dislikes { get; set; }


	}

}

