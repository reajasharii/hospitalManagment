using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// [Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Patient Account View (GET)
    [HttpGet]
    public async Task<IActionResult> PatientAccountView()
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

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PatientAccountView(EditAccountViewModel model)
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
                return RedirectToAction("PatientAccountView");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    
     
[HttpGet]
public async Task<IActionResult> ConnectToDoctor(string searchText, string specialty, bool? isAvailable)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return NotFound("Logged-in user not found.");
    }

    var doctorsQuery = _context.Doctors.AsQueryable();

    // Apply search filter for doctor's name
    if (!string.IsNullOrEmpty(searchText))
    {
        doctorsQuery = doctorsQuery.Where(d => d.FullName.Contains(searchText));
    }

    // Apply filter for specialty
    if (!string.IsNullOrEmpty(specialty))
    {
        doctorsQuery = doctorsQuery.Where(d => d.Specialty == specialty);
    }

    // Apply availability filter (Assume `IsAvailableForNewPatients` is a property indicating if the doctor is accepting new patients)
    

    var doctors = await doctorsQuery
        .Select(d => new DoctorViewModel
        {
            Doctor = d,
            IsConnected = _context.PatientDoctors
                .Any(pd => pd.DoctorId == d.Id && pd.PatientId == user.Id) // Check if connected
        })
        .ToListAsync();

    return View(doctors);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ConnectToDoctor(string doctorId)
{
    var patient = await _userManager.GetUserAsync(User);
    if (patient == null)
    {
        return NotFound("Logged-in user not found.");
    }

    var doctor = await _context.Doctors
        .FirstOrDefaultAsync(d => d.Id == doctorId);

    if (doctor != null)
    {
        var existingConnection = await _context.PatientDoctors
            .FirstOrDefaultAsync(pd => pd.PatientId == patient.Id && pd.DoctorId == doctor.Id);

        if (existingConnection != null)
        {
            TempData["ErrorMessage"] = "You are already connected to this doctor.";
            return RedirectToAction("ConnectToDoctor");
        }

        var patientDoctor = new PatientDoctor
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            PatientFullName = patient.FullName,
            DoctorFullName = doctor.FullName
        };

        _context.PatientDoctors.Add(patientDoctor);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "You are successfully connected to the doctor!";
        return RedirectToAction("ConnectToDoctor");
    }

    TempData["ErrorMessage"] = "Doctor not found.";
    return RedirectToAction("ConnectToDoctor");
}



 






   [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DisconnectFromDoctor(string doctorId)
{
   
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return NotFound("Logged-in user not found.");
    }


    var connection = await _context.PatientDoctors
        .FirstOrDefaultAsync(pd => pd.PatientId == user.Id && pd.DoctorId == doctorId);

    if (connection != null)
    {
        _context.PatientDoctors.Remove(connection);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "You have successfully disconnected from the doctor.";
    }

    return RedirectToAction("ConnectToDoctor");
}



    
}
