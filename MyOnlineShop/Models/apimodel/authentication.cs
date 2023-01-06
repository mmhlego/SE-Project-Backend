namespace MyOnlineShop.Models.apimodel
{
	public class LoginModel
	{
		public string username { get; set; }
		public string password { get; set; }
	}
	public class RegisterModel
	{
		public string type { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string phoneNumber { get; set; }
		public string email { get; set; }
		public DateTime birthDate { get; set; }
	}
}
