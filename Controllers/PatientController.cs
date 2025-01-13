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
public IActionResult ConnectToDoctor(string searchText)
{
    // Fetch all doctors or filter based on searchText
    var doctors = _context.Doctors
        .Where(d => string.IsNullOrEmpty(searchText) || d.FullName.Contains(searchText))
        .Select(d => new DoctorViewModel
        {
            Doctor = d,
            IsConnected = _context.PatientDoctors.Any(pd => pd.DoctorId == d.Id && pd.PatientId == _userManager.GetUserId(User))
        })
        .ToList();

    return View("ConnectToDoctor", doctors); // This will return the full view instead of a partial view
}




[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ConnectToDoctorPost(string doctorId)  // Changed method name to ConnectToDoctorPost
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

[HttpGet]
public async Task<IActionResult> ViewNotes()
{
    var patientId = _userManager.GetUserId(User); // Get the current logged-in patient's ID

    // Get the list of connected doctors for this patient
    var connectedDoctors = await _context.PatientDoctors
        .Where(pd => pd.PatientId == patientId) // Filter by patient
        .Select(pd => pd.DoctorId) // Get only the DoctorIds
        .ToListAsync();

    // Fetch notes for the connected doctors only
    var notes = await _context.Notes
        .Where(n => connectedDoctors.Contains(n.DoctorId) && n.PatientId == patientId) // Filter notes by connected doctors and the current patient
        .Select(n => new NoteViewModel
        {
            Content = n.Content,
            CreatedAt = n.CreatedAt
        })
        .ToListAsync();

    return View(notes);
}




    
}
