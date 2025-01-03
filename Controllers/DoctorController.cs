using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Ensure this is declared

        public DoctorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Ensure this is initialized
        }


  public async Task<IActionResult> ManagePatients()
    {
        var doctor = await _userManager.GetUserAsync(User);
        if (doctor == null)
        {
            return NotFound();
        }

        var patients = await _context.PatientDoctors
            .Where(pd => pd.DoctorId == doctor.Id)
            .Select(pd => pd.Patient)
            .ToListAsync();

        return View(patients);
    }
































        // GET: /Doctor/MyAccount
        [Authorize]
        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditAccountViewModel
            {
                FullName = user.FullName,
                Surname = user.Surname,
                Email = user.Email
            };

            return View(model);
        }

        // POST: /Doctor/MyAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyAccount(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.FullName;
                user.Surname = model.Surname;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Account updated successfully!";
                    return RedirectToAction("MyAccount");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
