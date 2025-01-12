using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    // Add this Email property
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

        // Generate password reset token
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Generate the reset password URL (this URL will be used in the email)
        var resetPasswordUrl = Url.Page(
            "/Account/ResetPassword", 
            pageHandler: null, 
            values: new { email = Email, token = token }, 
            protocol: Request.Scheme);

        // Send the reset link via email
        await _emailSender.SendEmailAsync(
            Email, 
            "Password Reset", 
            $"<p>Please reset your password using the following link:</p>" +
            $"<a href='{resetPasswordUrl}'>Reset your password</a>");

        // Show a confirmation message or redirect
        TempData["SuccessMessage"] = "Password reset email has been sent.";
        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
