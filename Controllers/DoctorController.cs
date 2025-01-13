using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; 

        public DoctorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }


public async Task<IActionResult> ManagePatients()
{
    // Get the current logged-in doctor
    var doctor = await _userManager.GetUserAsync(User);
    if (doctor == null)
    {
        return NotFound();
    }

    
    var patients = await _context.PatientDoctors
        .Where(pd => pd.DoctorId == doctor.Id)  
        .Include(pd => pd.Patient)  
        .Select(pd => pd.Patient)  
        .ToListAsync();

    
    return View(patients);  
}
        
        [Authorize]
public async Task<IActionResult> MyAccount()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return NotFound();
    }

    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == user.Id);
    if (doctor == null)
    {
        return NotFound();
    }

    var model = new EditAccountViewModel
    {
        FullName = doctor.FullName,
        Surname = doctor.Surname,
        Email = doctor.Email,
        DateOfBirth = doctor.DateOfBirth,
        Specialty = doctor.Specialty,
        LicenseNumber = doctor.LicenseNumber
    };

    return View(model);
}

        
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

        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == user.Id);
        if (doctor == null)
        {
            return NotFound();
        }

        doctor.FullName = model.FullName;
        doctor.Surname = model.Surname;
        doctor.Email = model.Email;
        doctor.DateOfBirth = (System.DateTime)model.DateOfBirth;
        doctor.Specialty = model.Specialty;
        doctor.LicenseNumber = model.LicenseNumber;

        _context.Update(doctor);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Account updated successfully!";
        return RedirectToAction("MyAccount");
    }

    return View(model);
}


[HttpGet]
public async Task<IActionResult> EditPatient(string id)
{
    var doctor = await _userManager.GetUserAsync(User);
    if (doctor == null)
    {
        return NotFound();
    }

 
    var patientDoctorConnection = await _context.PatientDoctors
        .FirstOrDefaultAsync(pd => pd.PatientId == id && pd.DoctorId == doctor.Id);

    if (patientDoctorConnection == null)
    {
        
        return Forbid(); 
    }

    
    var patient = await _context.Patients.FindAsync(id);
    if (patient == null)
    {
        return NotFound();
    }

   
    var model = new EditPatientViewModel
    {
        Id = patient.Id,
        FullName = patient.FullName,
        Surname = patient.Surname,
        Email = patient.Email,
        MedicalHistory = patient.MedicalHistory
    };

    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditPatient(EditPatientViewModel model)
{
    if (ModelState.IsValid)
    {
        var doctor = await _userManager.GetUserAsync(User);
        if (doctor == null)
        {
            return NotFound();
        }

        
        var patientDoctorConnection = await _context.PatientDoctors
            .FirstOrDefaultAsync(pd => pd.PatientId == model.Id && pd.DoctorId == doctor.Id);

        if (patientDoctorConnection == null)
 
            return Forbid(); 

        var patient = await _context.Patients.FindAsync(model.Id);
        if (patient == null)
        {
            return NotFound();
        }

        patient.FullName = model.FullName;
        patient.Surname = model.Surname;
        patient.Email = model.Email;
        patient.MedicalHistory = model.MedicalHistory;

        _context.Update(patient);
        await _context.SaveChangesAsync();

        return RedirectToAction("ManagePatients"); 
    }

    return View(model);
}


[HttpGet]
public async Task<IActionResult> DeletePatient(string id)
{
    var doctor = await _userManager.GetUserAsync(User);
    if (doctor == null)
    {
        return NotFound();
    }

  
    var patientDoctorConnection = await _context.PatientDoctors
        .FirstOrDefaultAsync(pd => pd.PatientId == id && pd.DoctorId == doctor.Id);

    if (patientDoctorConnection == null)
    {
      
        return Forbid(); 
    }

    // Fetch the patient's data
    var patient = await _context.Patients.FindAsync(id);
    if (patient == null)
    {
        return NotFound();
    }

    return View(patient);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeletePatientPost(string id)
{
    var doctor = await _userManager.GetUserAsync(User);
    if (doctor == null)
    {
        return NotFound();
    }

    var patientDoctorConnection = await _context.PatientDoctors
        .FirstOrDefaultAsync(pd => pd.PatientId == id && pd.DoctorId == doctor.Id);

    if (patientDoctorConnection == null)
    {
        return Forbid(); 
    }

   
    var patient = await _context.Patients.FindAsync(id);
    if (patient == null)
    {
        return NotFound();
    }

  
    _context.PatientDoctors.Remove(patientDoctorConnection);
    
    
    _context.Patients.Remove(patient);
    await _context.SaveChangesAsync();

    return RedirectToAction("ManagePatients"); 
}






[HttpGet]
public IActionResult CreatePatient()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreatePatient(CreatePatientViewModel model)
{
    if (ModelState.IsValid)
    {
    
        var patient = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Surname = model.Surname,
            MedicalHistory = model.MedicalHistory 
        };

        var result = await _userManager.CreateAsync(patient, model.Password);
        if (result.Succeeded)
        {
            
            var doctor = await _userManager.GetUserAsync(User);
            if (doctor != null)
            {
                var patientDoctorConnection = new PatientDoctor
                {
                    PatientId = patient.Id,
                    DoctorId = doctor.Id,
                    PatientFullName = patient.FullName,
                    DoctorFullName = doctor.FullName
                };

                _context.PatientDoctors.Add(patientDoctorConnection);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Patient created successfully!";
            return RedirectToAction("ManagePatients");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    return View(model);
}






[HttpGet]
public async Task<IActionResult> SendNote()
{
    var doctorId = _userManager.GetUserId(User); // Get the current logged-in doctor's ID

    // Get the list of patients connected to the doctor
    var patients = await _context.PatientDoctors
        .Where(pd => pd.DoctorId == doctorId)  // Filter by the current doctor's ID
        .Select(pd => new SelectListItem
        {
            Value = pd.PatientId,
            Text = pd.PatientFullName
        })
        .ToListAsync();

    // Pass the list of connected patients to the view
    ViewBag.Patients = new SelectList(patients, "Value", "Text");

    return View();
}



[HttpPost]
public async Task<IActionResult> SendNote(NoteViewModel model)
{
    // Ensure DoctorId is correctly set before any logic
    var doctorId = _userManager.GetUserId(User); // Get the DoctorId from the current logged-in user

    try
    {
        if (ModelState.IsValid)
        {
            // Ensure that PatientId and Content are set properly
            if (string.IsNullOrEmpty(model.PatientId) || string.IsNullOrEmpty(model.Content))
            {
                TempData["ErrorMessage"] = "Patient or Note content is missing.";
                return View(model); // Return to the same page with error message
            }

            // Create and save the note
            var note = new Note
            {
                DoctorId = doctorId, // Use the doctorId we retrieved
                PatientId = model.PatientId,
                Content = model.Content,
                CreatedAt = DateTime.Now
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            // Success message
            TempData["SuccessMessage"] = "Note sent successfully!";
            return RedirectToAction("SendNote"); // Redirect to avoid resubmission on page reload
        }

        // Log validation errors if any
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }

        // If validation fails, reload patient list and show error message
        var patients = await _context.PatientDoctors
            .Where(pd => pd.DoctorId == doctorId)
            .Select(pd => new SelectListItem
            {
                Value = pd.PatientId,
                Text = pd.PatientFullName
            })
            .ToListAsync();

        ViewBag.Patients = new SelectList(patients, "Value", "Text");

        TempData["ErrorMessage"] = "Failed to send the note. Please try again.";
        return View(model);
    }
    catch (Exception ex)
    {
        // Handle unexpected errors
        TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
        return View(model);
    }
}



























    }
}
