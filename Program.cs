using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;







// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configure DbContext and Identity (same as before)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Enforce account confirmation for sign-in
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
// builder.Services.AddScoped<MySeedData>();

var app = builder.Build();

// Use authentication and authorization middleware
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     try
//     {
//         var seedData = services.GetRequiredService<MySeedData>();
//         await seedData.Initialize(); // Call seeding logic here
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Error during database seeding: {ex.Message}");
//     }
// }



// Routing setup
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();
app.Run();
