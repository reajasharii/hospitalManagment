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































    }
}
