using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalManagement.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.Services;
using HospitalManagement.Models;

namespace HospitalManagement.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [Required]
            [Display(Name = "Medical History")]
            public string MedicalHistory { get; set; } = "No medical history available";

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "The password must be at least 6 characters long.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            // Fetch external login providers (like Google, Facebook)
            ExternalLogins = (await _authenticationSchemeProvider.GetAllSchemesAsync())
                             .Where(s => s.DisplayName != null)
                             .ToList();
        }

       public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    returnUrl ??= Url.Content("~/");

    if (ModelState.IsValid)
    {
        var user = new ApplicationUser 
        { 
            UserName = Input.Email, 
            Email = Input.Email, 
            FullName = Input.FullName, 
            Surname = Input.Surname, 
            MedicalHistory = Input.MedicalHistory 
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");

            // Assign the "Patient" role to the user
            var roleResult = await _userManager.AddToRoleAsync(user, "Patient");

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // If adding the role fails, delete the user to avoid inconsistent state
                await _userManager.DeleteAsync(user);
                return Page();
            }

            // Automatically sign in the user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect to the returnUrl (if provided) or to the home page
            return LocalRedirect(returnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    // Re-fetch external logins if validation fails
    ExternalLogins = (await _authenticationSchemeProvider.GetAllSchemesAsync())
                     .Where(s => s.DisplayName != null)
                     .ToList();

    return Page();
}

    }
}
