namespace MyOnlineShop.Services
{
	public class Logger
	{
		public static void LoggerFunc(string ApiRoute, Guid userID, object Data_func)
		{
			DateTime date = DateTime.Now;
			string path = @"./SystemLog.txt";
			string Log = $"{date.Date}\t|\t{date.TimeOfDay}\t|\t{ApiRoute}\t|\t{userID}\t|\t{System.Text.Json.JsonSerializer.Serialize(Data_func)}";

			StreamWriter LogWriter = new StreamWriter(path, true);

			LogWriter.WriteLine(Log);
			LogWriter.Close();
		}
	}
}
