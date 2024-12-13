using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity.UI.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true) // Set to false to not require confirmed email
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();






var app = builder.Build();

var _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();  
app.UseAuthorization();   


app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    _logger.LogInformation("[DEBUG] Request Path: {Path}, Authenticated: {IsAuthenticated}", path, context.User.Identity?.IsAuthenticated);

   
    if (context.User.Identity?.IsAuthenticated ?? false)
    {
        
        if (path.StartsWith("/Identity/Account/Login") && !path.Contains("ResetPassword"))
        {
            _logger.LogInformation("[DEBUG] Redirecting authenticated user to Home from Login page.");
            context.Response.Redirect("/"); 
            return;
        }
    }
    else
    {
       
        if (path.StartsWith("/Identity/Account/Login") || 
            path.StartsWith("/Identity/Account/Register") || 
            path.StartsWith("/Identity/Account/ForgotPassword") ||
            path.StartsWith("/Identity/Account/ResetPassword") || 
            path.StartsWith("/Identity/Account/ResendEmailConfirmation") ||
            path.StartsWith("/Identity/Account/ConfirmEmail"))
        {
            await next();
            return;
        }

    
        _logger.LogInformation("[DEBUG] Redirecting unauthenticated user to Login page.");
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }

    _logger.LogInformation("[DEBUG] No redirection needed, proceeding to next middleware.");
    await next();
});



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); 

app.Run();
