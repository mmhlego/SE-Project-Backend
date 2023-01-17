namespace MyOnlineShop.Services
{
	public class Logger
	{
		public static void LoggerFunc(string ApiRoute, Guid userID, object inData, object outData)
		{
			DateTime date = DateTime.Now;
			string path = @"./SystemLog.txt";
			string Log = $"{date.Date}\t|\t{date.TimeOfDay}\t|\t{ApiRoute}\t|\t{userID}\t|\t{System.Text.Json.JsonSerializer.Serialize(inData)}\t|\t{System.Text.Json.JsonSerializer.Serialize(outData)}";

			StreamWriter LogWriter = new StreamWriter(path, true);

			LogWriter.WriteLine(Log);
			LogWriter.Close();
		}
	}
}
