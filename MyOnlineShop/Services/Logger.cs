namespace MyOnlineShop.Services
{
	public class Logger
	{
		public static void LoggerFunc(string user, string type, string func)
		{
			string path = @"./SystemLog.txt";
			string Log = "User : " + user + " Function : " + func + " Type : " + type;

			StreamWriter LogWriter = new StreamWriter(path, true);

			LogWriter.WriteLine(Log);
			LogWriter.Close();
		}
	}
}
