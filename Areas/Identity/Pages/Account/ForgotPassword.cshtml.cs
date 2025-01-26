using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Configuration;

public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

  public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration)
{
    _userManager = userManager;
    _emailSender = emailSender;
    _configuration = configuration;
}

    
    [BindProperty]
    public string Email { get; set; }

     public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Email))
        {
            ModelState.AddModelError(string.Empty, "Please enter your email.");
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "No user found with this email.");
            return Page();
        }

     
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

 
        var fixedEmail = _configuration["Smtp:User"];

      
        var resetPasswordUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { email = Email, token = token },
            protocol: Request.Scheme);

   
        await _emailSender.SendEmailAsync(
            fixedEmail,
            "Password Reset",
            $"<p>Password reset requested for user: <strong>{Email}</strong>.</p>" +
            $"<p>Click the following link to reset the password:</p>" +
            $"<a href='{resetPasswordUrl}'>Reset your password</a>");

        
        TempData["SuccessMessage"] = "Password reset email has been sent.";
        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
