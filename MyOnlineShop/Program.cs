using Microsoft.EntityFrameworkCore;
using MyOnlineShop.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Db Context

builder.Services.AddDbContext<MyShopContext>(options =>
{
	//options.UseSqlServer("Data Source =DESKTOP-TU89R0L\\EFI;Initial Catalog=SevenShop_DB;Integrated Security=true;Trust Server Certificate=true;");
	//options.UseSqlServer("Data Source=localhost,1433; Database=shopdatabase5; User Id=sa; Password=someThingComplicated1234; Trust Server Certificate=true;");
	options.UseSqlServer("Data Source=KARIMI-PC;Initial Catalog=MyOnlineShop_DB;Integrated Security=false;User ID=sa;Password=5291431220;Trust Server Certificate=true;");
});

#endregion
#region Authentication

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{

		options.LoginPath = "/auth/Login";
		options.LogoutPath = "/auth/Logout";
		options.ExpireTimeSpan = TimeSpan.FromDays(4);
	})
  ;


#endregion



builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.Use(async (context, next) => { await next();
//    if (context.Response.StatusCode == 404)
//    { context.Response.Redirect('#/components/responses/NotFound'); } });
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
