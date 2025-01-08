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

// DoctorController.cs
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

public IActionResult ConnectToDoctor()
{
    // Fetch unique specialties
    var specialties = _context.Doctors
        .Select(d => d.Specialty)
        .Distinct()
        .ToList();

    ViewBag.Specialties = specialties;

    // Fetch all doctors for the initial list
    var doctors = _context.Doctors.ToList();
    return View(doctors);
}





[HttpPost]
public async Task<IActionResult> FilterDoctors([FromBody] DoctorFilterViewModel filters)
{
    var query = _context.Doctors.AsQueryable();

    // Apply search text filter
    if (!string.IsNullOrEmpty(filters.SearchText))
    {
        query = query.Where(d => d.FullName.Contains(filters.SearchText) ||
                                 d.Email.Contains(filters.SearchText));
    }

    // Apply specialty filter
    if (!string.IsNullOrEmpty(filters.Specialty))
    {
        query = query.Where(d => d.Specialty == filters.Specialty);
    }

    // Apply checkbox filters
    if (filters.HasLicense)
    {
        query = query.Where(d => !string.IsNullOrEmpty(d.LicenseNumber));
    }

    if (filters.EmailConfirmed)
    {
        query = query.Where(d => d.EmailConfirmed);
    }

    // Fetch filtered doctors
    var filteredDoctors = await query.ToListAsync();

    // Return as JSON for frontend
    return Json(filteredDoctors.Select(d => new
    {
        d.FullName,
        d.Specialty,
        d.Email,
        d.LicenseNumber
    }));
}








































    }
}
