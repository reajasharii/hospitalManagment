using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();


builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<MySeedData>();
builder.Services.AddLogging(); 

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seedData = services.GetRequiredService<MySeedData>();
    await seedData.Initialize(); 
}


app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    await next.Invoke();
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "admin-deletepatient",
    pattern: "Admin/DeletePatient/{id?}",
    defaults: new { controller = "Admin", action = "DeletePatient" });

app.MapControllerRoute(
    name: "admin-deleteconfirmedpatient",
    pattern: "Admin/DeleteConfirmedPatient/{id}",
    defaults: new { controller = "Admin", action = "DeleteConfirmedPatient" });


app.MapControllerRoute(
    name: "admin-editpatient",
    pattern: "Admin/EditPatient/{id?}",
    defaults: new { controller = "Admin", action = "EditPatient" });


app.MapRazorPages();


app.MapFallback(context =>
{
    Console.WriteLine($"Unhandled request: {context.Request.Path}");
    return Task.CompletedTask;
});

app.Run();
