using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;

namespace MyOnlineShop.Services
{
	public class Logger
	{
		public static void LoggerFunc(DateTime date, string ApiRoute, Guid userID, object Data_func)
		{
			string path = @"./SystemLog.txt";
			string Log = $"{date.Date}\t|\t{date.TimeOfDay}\t|\t{ApiRoute}\t|\t{userID}\t|\t{Data_func}" ;

			StreamWriter LogWriter = new StreamWriter(path, true);

			LogWriter.WriteLine(Log);
			LogWriter.Close();
		}
	}
}
