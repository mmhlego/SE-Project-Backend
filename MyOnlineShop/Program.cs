using Microsoft.EntityFrameworkCore;
using MyOnlineShop.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

#region Db Context

builder.Services.AddDbContext<MyShopContext>(options =>
{
	//options.UseSqlServer("Data Source =DESKTOP-TU89R0L\\EFI;Initial Catalog=MyOnlineShop_DB2;Integrated Security=true;Trust Server Certificate=true;");
	//options.UseSqlServer("Data Source=localhost,1433; Database=shopdatabase5; User Id=sa; Password=someThingComplicated1234; Trust Server Certificate=true;");
	// options.UseSqlServer("Data Source=KARIMI-PC;Initial Catalog=MyOnlineShop_DB;Integrated Security=false;User ID=sa;Password=5291431220;Trust Server Certificate=true;");
	options.UseSqlServer("Data Source=MMHLEGO-PC;Initial Catalog=MyOnlineShop_DB1;Integrated Security=true;Trust Server Certificate=true;");
});

#endregion
#region Authentication

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.SameSite = SameSiteMode.None;
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


app.UseCors(builder => builder
	.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://localhost", "https://localhost")
	.AllowCredentials()
	.AllowAnyMethod()
	.AllowAnyHeader()
	);

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
