using System;
using System.Linq;
using System.Threading.Tasks;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static HospitalManagement.ViewModels.CreatePatientViewModel;


namespace HospitalManagement.Controllers
{
   
    public class AdminController : Controller
    {
            private readonly ApplicationDbContext _context;
         private readonly UserManager<ApplicationUser> _userManager;   
         

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }





    
        public async Task<IActionResult> ManageAdmins()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return View("ManageAdmins", admins);
        }

     
        public IActionResult CreateAdmin()
        {
            return View("CreateAdmin");
        }

      
 [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateAdmin(CreateAdminViewModel model)
{
    if (ModelState.IsValid)
    {
        var admin = new ApplicationUser
        {
            FullName = model.FullName,
            Surname = model.Surname,
            Email = model.Email,
            UserName = model.Email
        };

        var result = await _userManager.CreateAsync(admin, model.Password);

        if (result.Succeeded)
        {
           
            await _userManager.AddToRoleAsync(admin, "Admin");
            return RedirectToAction(nameof(ManageAdmins));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    return View(model);
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





        
public async Task<IActionResult> EditAdmin(string id)
{
    if (id == null)
    {
        return NotFound();
    }

    var admin = await _userManager.FindByIdAsync(id);
    if (admin == null)
    {
        return NotFound();
    }

    var model = new EditAdminViewModel
    {
        Id = admin.Id,
        FullName = admin.FullName,
        Email = admin.Email,
        Password = "",  
        ConfirmPassword = ""  
    };

    return View(model);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditAdmin(EditAdminViewModel model)
{
    if (ModelState.IsValid)
    {
        var admin = await _userManager.FindByIdAsync(model.Id);
        if (admin == null)
        {
            return NotFound();
        }

        admin.FullName = model.FullName;
        admin.Email = model.Email;

     
        if (!string.IsNullOrEmpty(model.Password))
        {
            var removePasswordResult = await _userManager.RemovePasswordAsync(admin);
            if (removePasswordResult.Succeeded)
            {
                var addPasswordResult = await _userManager.AddPasswordAsync(admin, model.Password);
                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            else
            {
                foreach (var error in removePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        var result = await _userManager.UpdateAsync(admin);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Admin updated successfully!";
            return RedirectToAction("ManageAdmins");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }

    return View(model);
}



public async Task<IActionResult> DeleteAdmin(string id)
{
    if (id == null)
    {
        return NotFound();
    }

    var admin = await _userManager.FindByIdAsync(id);
    if (admin == null)
    {
        return NotFound();
    }

    return View(admin); 
}


[HttpPost, ActionName("DeleteAdmin")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmedAdmin(string id)
{
    if (id == null)
    {
        return NotFound();
    }

    var admin = await _userManager.FindByIdAsync(id);
    if (admin == null)
    {
        return NotFound();
    }

    var result = await _userManager.DeleteAsync(admin);
    if (result.Succeeded)
    {
        return RedirectToAction(nameof(ManageAdmins)); 
    }

   
    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }

    return View(admin); 
}


        
    public async Task<IActionResult> ManageDoctors()
{
    var doctors = await _context.Doctors.ToListAsync();
    return View(doctors); 
}


   
    public IActionResult CreateDoctor()
    {
        return View();
    }

    
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateDoctor(CreateDoctorViewModel model)
{
    if (ModelState.IsValid)
    {
       
        
        var doctorUser = new ApplicationUser
        {
            FullName = model.FullName,
            Email = model.Email,
            UserName = model.Email,
            Surname = model.Surname,
            
        };

      
        var result = await _userManager.CreateAsync(doctorUser, model.Password);

        if (result.Succeeded)
        {
            
            await _userManager.AddToRoleAsync(doctorUser, "Doctor");

           
            var doctor = new Doctor
            {
                Id = doctorUser.Id,  
                FullName = model.FullName,
                Specialty = model.Specialty,
                LicenseNumber = model.LicenseNumber,
                Surname = model.Surname,
                 Email = model.Email,
           
            };

            
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageDoctors)); 
        }

        
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    return View(model); 
}




[HttpGet]
public IActionResult EditDoctor(string doctorid)
{
    var doctor = _context.Doctors.Find(doctorid);
    if (doctor == null) return NotFound();

    var viewModel = new EditDoctorViewModel
    {
        Id = doctor.Id,
        FullName = doctor.FullName,
        Surname = doctor.Surname,
        LicenseNumber = doctor.LicenseNumber,
        Specialty = doctor.Specialty,
         Email = doctor.Email,

        
    };

    return View(viewModel);
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult EditDoctor(EditDoctorViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    var doctor = _context.Doctors.Find(model.Id);
    if (doctor == null) return NotFound();

    doctor.FullName = model.FullName;
    doctor.Surname = model.Surname;
    doctor.Specialty = model.Specialty;
    doctor.LicenseNumber = model.LicenseNumber;
      doctor.Email = model.Email; 
    

    _context.SaveChanges();

   
    return RedirectToAction("ManageDoctors");
}







  
 
   
    public async Task<IActionResult> DeleteDoctor(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var doctor = await _context.Doctors.FirstOrDefaultAsync(m => m.Id == id);
        if (doctor == null)
        {
            return NotFound();
        }

        return View(doctor); 
    }

   
   [HttpPost, ActionName("DeleteDoctor")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmedDoctor(string id)
{
    if (id == null)
    {
        return NotFound();
    }

    var doctor = await _context.Doctors.FindAsync(id);
    if (doctor != null)
    {
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(ManageDoctors));
}




        
public async Task<IActionResult> ManagePatients()
{
    // Get users in the "Patient" role
    var patients = await _userManager.GetUsersInRoleAsync("Patient");
    return View("ManagePatients", patients); // Return to view with patient data
}


    public IActionResult CreatePatient()
    {
    return View("CreatePatient");
    }

 [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreatePatient(CreatePatientViewModel model)
{
    if (ModelState.IsValid)
    {
        // Map Patient to ApplicationUser
        var patientUser = new ApplicationUser
        {
            FullName = model.FullName,
            Surname = model.Surname,
            Email = model.Email,
            UserName = model.Email,
            MedicalHistory = model.MedicalHistory, // Username is the email
        };

        // Use UserManager to create ApplicationUser
        var result = await _userManager.CreateAsync(patientUser, model.Password);

        if (result.Succeeded)
        {
            // Assign the Patient role
            await _userManager.AddToRoleAsync(patientUser, "Patient");

            // Add a new Patient to the database linked by Id
            var patient = new Patient
            {
                Id = patientUser.Id, // Use ApplicationUser ID as foreign key
                FullName = model.FullName,
                Surname = model.Surname,
                Email = model.Email,
                MedicalHistory = model.MedicalHistory
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Redirect to ManagePatients view
            return RedirectToAction("ManagePatients");
        }

        // Handle errors
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    return View(model);
}


    // GET: Edit Patient
 // GET: Edit Patient
public async Task<IActionResult> EditPatient(string id)
{
    if (string.IsNullOrEmpty(id))
    {
        return NotFound(); // Return a 404 if no id is provided
    }

    var patient = await _userManager.FindByIdAsync(id); // Fetch the patient by their ID
    if (patient == null)
    {
        return NotFound(); // Return a 404 if the patient is not found
    }

    // Map the patient data to the EditPatientViewModel
    var model = new EditPatientViewModel
    {
        Id = patient.Id,
        FullName = patient.FullName,
        Surname = patient.Surname,
        Email = patient.Email,
        MedicalHistory = patient.MedicalHistory
    };

    return View(model); // Return the view with the patient details
}



// POST: Edit Patient
 // POST: Edit Patient
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditPatient(EditPatientViewModel model)
{
    if (ModelState.IsValid)
    {
        var patient = await _userManager.FindByIdAsync(model.Id); // Find patient by Id
        if (patient == null)
        {
            return NotFound(); // If patient doesn't exist, return 404
        }

        // Update patient details
        patient.FullName = model.FullName;
        patient.Surname = model.Surname;
        patient.Email = model.Email;
        patient.MedicalHistory = model.MedicalHistory;

        // Update patient record in database
        var result = await _userManager.UpdateAsync(patient);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Patient updated successfully!";
            return RedirectToAction("ManagePatients"); // Redirect back to ManagePatients
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description); // Show error if update fails
            }
        }
    }

    return View(model); // If model is invalid, return the view with current data
}



       
       
       
       

 // GET: Delete Patient
 [HttpGet]
    public async Task<IActionResult> DeletePatient(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var patient = await _userManager.FindByIdAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        return View(patient);
    }

   
    public async Task<IActionResult> DeleteConfirmedPatient(string id)
{
    var patient = await _userManager.FindByIdAsync(id);

    if (patient == null)
    {
        TempData["ErrorMessage"] = "Patient not found.";
        return RedirectToAction("ManagePatients");
    }

    
    var patientDoctors = _context.PatientDoctors.Where(pd => pd.PatientId == patient.Id);
    _context.PatientDoctors.RemoveRange(patientDoctors);


    await _context.SaveChangesAsync();

 
    var result = await _userManager.DeleteAsync(patient);

    if (result.Succeeded)
    {
        TempData["SuccessMessage"] = "Patient deleted successfully.";
    }
    else
    {
        TempData["ErrorMessage"] = "Error occurred while deleting the patient.";
    }

    return RedirectToAction("ManagePatients");
}





}
}