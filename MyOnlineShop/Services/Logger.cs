using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Models.apimodel;
using System.Security.Claims;
using MyOnlineShop.Models;
using MyOnlineShop.Services;

namespace MyOnlineShop.Services
{
	public class Logger
    {
        private MyShopContext _context;
        public Logger(MyShopContext context)
        {
            _context = context;
        }
        public void LoggerFunc(string ApiRoute, object inData, object outData, ClaimsPrincipal user)
		{
            Guid? userID = _context.users.FirstOrDefault(l => l.UserName == user.FindFirstValue(ClaimTypes.Name))?.ID;
            LoggerFunc(ApiRoute, inData, outData, userID);
        }
        
        public void LoggerFunc(string ApiRoute, object inData, object outData, Guid? userID)
		{
			try
			{
				string Log;
                DateTime date = DateTime.Now;
                string path = @"./SystemLog.txt";
                if (userID is null)
				{
                    Log = $"{date.Date}\t|\t{date.TimeOfDay}\t|\t{ApiRoute}\t|\tAnonymous\t|\t{System.Text.Json.JsonSerializer.Serialize(inData)}\t|\t{System.Text.Json.JsonSerializer.Serialize(outData)}";
				}
				else
				{
                    Log = $"{date.Date}\t|\t{date.TimeOfDay}\t|\t{ApiRoute}\t|\t{userID}\t|\t{System.Text.Json.JsonSerializer.Serialize(inData)}\t|\t{System.Text.Json.JsonSerializer.Serialize(outData)}";
                }
                StreamWriter LogWriter = new StreamWriter(path, true);
				LogWriter.WriteLine(Log);
				LogWriter.Close();
			}
			catch
			{
            }
		}
	}
}
