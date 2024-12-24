using System;
using System.Linq;
using System.Threading.Tasks;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


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

        // Manage Admins - List all users with Admin role
        public async Task<IActionResult> ManageAdmins()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return View("ManageAdmins", admins);
        }

        // Create Admin - GET
        public IActionResult CreateAdmin()
        {
            return View("CreateAdmin");
        }

        // Create Admin - POST
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
            // Assign the Admin role
            await _userManager.AddToRoleAsync(admin, "Admin");
            return RedirectToAction(nameof(ManageAdmins));
        }

        // Handle errors
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    return View(model);
}








        // Edit Admin - GET// Edit Admin - GET

// Edit Admin - GET
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
        Password = "",  // Empty password field
        ConfirmPassword = ""  // Empty confirm password field
    };

    return View(model);
}

// Edit Admin - POST
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

        // Update fields
        admin.FullName = model.FullName;
        admin.Email = model.Email;

        // Handle password update if provided
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

        // Update the admin user
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



        // Edit Admin - POST
       



    // Delete Admin - GET (show confirmation)
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

    return View(admin); // Pass the admin to the confirmation view
}

// Delete Admin - POST (actual deletion)
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
        return RedirectToAction(nameof(ManageAdmins)); // Redirect to ManageAdmins after successful deletion
    }

    // If there are errors, add them to the ModelState and return to the confirmation view
    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }

    return View(admin); // If there was an error, return the admin to the confirmation page
}


        // CRUD Operations for Doctors
    public async Task<IActionResult> ManageDoctors()
{
    var doctors = await _context.Doctors.ToListAsync();
    return View(doctors); // Return the list of doctors to the view
}


    // Create Doctor - GET (Show the form to create a new doctor)
    public IActionResult CreateDoctor()
    {
        return View();
    }

    // Create Doctor - POST (Save the new doctor to the database)
 // Create Doctor - POST (Save the new doctor to the database)
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateDoctor(CreateDoctorViewModel model)
{
    if (ModelState.IsValid)
    {
        // Create ApplicationUser for authentication
        
        var doctorUser = new ApplicationUser
        {
            FullName = model.FullName,
            Email = model.Email,
            UserName = model.Email,
            Surname = model.Surname,
             // Use email as the username for the user
        };

        // Create the ApplicationUser in Identity system
        var result = await _userManager.CreateAsync(doctorUser, model.Password);

        if (result.Succeeded)
        {
            // Add the Doctor role
            await _userManager.AddToRoleAsync(doctorUser, "Doctor");

            // Now create the Doctor entity (separate from ApplicationUser)
            var doctor = new Doctor
            {
                Id = doctorUser.Id,  // Link to ApplicationUser via Id
                FullName = model.FullName,
                Specialty = model.Specialty,
                LicenseNumber = model.LicenseNumber,
                Surname = model.Surname,
           
            };

            // Add Doctor to the database
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageDoctors));  // Redirect after successful creation
        }

        // Handle errors during user creation
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    return View(model);  // Return view with validation errors if any
}



 // Edit Doctor Action
// Edit Doctor - GET
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
        Specialty = doctor.Specialty
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

    _context.SaveChanges();

    // After successful edit, redirect to ManageDoctors
    return RedirectToAction("ManageDoctors");
}





// Edit Doctor - POST
// [HttpPost]
// public IActionResult EditDoctor(EditDoctorViewModel model)
// {
//     if (!ModelState.IsValid) return View(model);

//     var doctor = _context.Doctors.Find(model.Id);
//     if (doctor == null) return NotFound();

//     doctor.FullName = model.FullName;
//     doctor.Surname = model.Surname;
//     doctor.LicenseNumber = model.LicenseNumber;
//     doctor.Specialization = model.Specialization;

//     _context.SaveChanges();

//     return RedirectToAction("Index");
// }

  
 
    // Delete Doctor - GET (Show confirmation to delete a doctor)
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

        return View(doctor); // Pass the doctor to the delete confirmation view
    }

    // Delete Doctor - POST (Perform the deletion of a doctor)
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




        // CRUD Operations for Patients
        public async Task<IActionResult> ManagePatients()
        {
            var patients = await _context.Patients.ToListAsync();
            return View("/Views/ManagePatients.cshtml", patients);
        }

        public IActionResult CreatePatient()
        {
            return View("/Views/ManagePatients/CreatePatient.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient([Bind("FullName,Surname,DateOfBirth,MedicalHistory")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManagePatients));
            }
            return View("/Views/ManagePatients/CreatePatient.cshtml", patient);
        }

        public async Task<IActionResult> EditPatient(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View("/Views/ManagePatients/EditPatient.cshtml", patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(string id, [Bind("Id,FullName,Surname,DateOfBirth,MedicalHistory")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Patients.Any(e => e.Id == patient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManagePatients));
            }
            return View("/Views/ManagePatients/EditPatient.cshtml", patient);
        }

        public async Task<IActionResult> DeletePatient(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View("/Views/ManagePatients/DeletePatient.cshtml", patient);
        }

        [HttpPost, ActionName("DeletePatient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedPatient(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManagePatients));
        }
    }
}
