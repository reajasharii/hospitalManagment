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
    
    var patients = await _context.Patients.ToListAsync();

  
    return View("ManagePatients", patients);
}


    public IActionResult CreatePatient()
    {
    return View("CreatePatient");
    }

 [HttpPost]
public async Task<IActionResult> CreatePatient(CreatePatientViewModel model)
{
    if (ModelState.IsValid)
    {
        var patient = new Patient
        {
            FullName = model.FullName,
            Surname = model.Surname,
            Email = model.Email,
            MedicalHistory = model.MedicalHistory,
           
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

       
        return RedirectToAction("ManagePatients");
    }
    
    
    return View(model);
}
    [HttpGet]
public async Task<IActionResult> EditPatient(string id)
{
    
    var patient = await _context.Patients.FindAsync(id);
    
    if (patient == null)
    {
        return NotFound();
    }

    var model = new EditPatientViewModel
    {
        FullName = patient.FullName,
        Surname = patient.Surname,
        Email = patient.Email,
        MedicalHistory = patient.MedicalHistory
    };

    return View(model);
}

      [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditPatient(string id, EditPatientViewModel model)
{
    if (ModelState.IsValid)
    {
        var patient = await _context.Patients.FindAsync(id);
        
        if (patient == null)
        {
            return NotFound();
        }

        
        patient.FullName = model.FullName;
        patient.Surname = model.Surname;
        patient.Email = model.Email;
        patient.MedicalHistory = model.MedicalHistory;

       
        await _context.SaveChangesAsync();

        
        return RedirectToAction("ManagePatients");
    }

    
    return View(model);
}
     

       
       
       
       

[HttpGet]
public async Task<IActionResult> DeletePatient(string id)
{
    var patient = await _context.Patients.FindAsync(id);

    if (patient == null)
    {
        return NotFound();
    }

    return View(patient);  
}





[HttpPost, ActionName("DeletePatient")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeletePatientConfirmed(string id)
{
    var patient = await _context.Patients.FindAsync(id);

    if (patient == null)
    {
        return NotFound();
    }

    _context.Patients.Remove(patient);
    await _context.SaveChangesAsync();

    return RedirectToAction("ManagePatients"); 
}




    }
}
