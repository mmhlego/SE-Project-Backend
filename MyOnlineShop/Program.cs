using Microsoft.EntityFrameworkCore;
using MyOnlineShop.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Db Context

builder.Services.AddDbContext<MyShopContex>(options => {
    options.UseSqlServer("Data Source =DESKTOP-TU89R0L\\EFI;Initial Catalog=OnlineShop_Db;Integrated Security=true; Trust Server Certificate=true;");
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
