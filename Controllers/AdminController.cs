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
        private readonly ILogger<AdminController> _logger;


        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
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

                var patientUser = new ApplicationUser
                {
                    FullName = model.FullName,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(patientUser, model.Password);

                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(patientUser, "Patient");


                    var patient = new Patient
                    {
                        Id = patientUser.Id,
                        FullName = model.FullName,
                        Surname = model.Surname,
                        Email = model.Email,
                        MedicalHistory = model.MedicalHistory
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();


                    return RedirectToAction("ManagePatients");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }



        public async Task<IActionResult> EditPatient(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var patient = await _userManager.FindByIdAsync(id);
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
                var patient = await _userManager.FindByIdAsync(model.Id);
                if (patient == null)
                {
                    return NotFound();
                }


                patient.FullName = model.FullName;
                patient.Surname = model.Surname;
                patient.Email = model.Email;
                patient.MedicalHistory = model.MedicalHistory;


                var result = await _userManager.UpdateAsync(patient);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Patient updated successfully!";
                    return RedirectToAction("ManagePatients");
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
        public async Task<IActionResult> ManageConnections()
        {

            var connections = await _context.PatientDoctors
                .Select(pd => new ConnectionViewModel
                {
                    DoctorId = pd.DoctorId,
                    DoctorName = pd.DoctorFullName,
                    PatientName = pd.PatientFullName
                })
                .ToListAsync();


            var viewModel = new ManageConnectionsViewModel
            {
                Connections = connections,
                AvailableDoctors = await _context.Doctors.ToListAsync(),
                CurrentPatients = await _userManager.Users.ToListAsync()
            };

            return View(viewModel);
        }









        public async Task<IActionResult> AddConnection(string selectedPatientId, string doctorId)
        {
            var patient = await _context.Users.FindAsync(selectedPatientId);  // Patient
            var doctor = await _context.Doctors.FindAsync(doctorId);  // Doctor

            if (patient == null || doctor == null)
            {
                TempData["ErrorMessage"] = "Patient or Doctor not found.";
                return RedirectToAction("ManageConnections");
            }

            var existingConnection = await _context.PatientDoctors
                .FirstOrDefaultAsync(pd => pd.PatientId == selectedPatientId && pd.DoctorId == doctorId);

            if (existingConnection != null)
            {
                TempData["ErrorMessage"] = "This connection already exists.";
                return RedirectToAction("ManageConnections");
            }

            var connection = new PatientDoctor
            {
                PatientId = selectedPatientId,
                DoctorId = doctorId,
                PatientFullName = patient.UserName,
                DoctorFullName = doctor.FullName,
            };


            _context.PatientDoctors.Add(connection);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Connection added successfully.";
            return RedirectToAction("ManageConnections");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConnection(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                TempData["ErrorMessage"] = "Invalid Doctor ID.";
                return RedirectToAction("ManageConnections");
            }

            var connection = await _context.PatientDoctors
                .FirstOrDefaultAsync(pd => pd.DoctorId == doctorId);

            if (connection == null)
            {
                TempData["ErrorMessage"] = "Connection not found.";
                return RedirectToAction("ManageConnections");
            }


            _context.PatientDoctors.Remove(connection);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Connection successfully deleted.";
            return RedirectToAction("ManageConnections");
        }












        // [HttpGet]
        // public async Task<IActionResult> ManageConnections()
        // {
        //     var user = await _userManager.GetUserAsync(User); // Get the current patient
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     // Get the current patient’s connections
        //     var patientConnections = await _context.PatientDoctors
        //         .Include(pd => pd.Doctor)
        //         .Where(pd => pd.PatientId == user.Id)
        //         .ToListAsync();

        //     var model = new ManageConnectionsViewModel
        //     {
        //         Connections = patientConnections.Select(c => new ConnectionViewModel
        //         {
        //             DoctorId = c.Doctor.Id,
        //             DoctorName = $"{c.Doctor.FullName} {c.Doctor.Surname}",
        //             Specialty = c.Doctor.Specialty,
        //             LicenseNumber = c.Doctor.LicenseNumber
        //         }).ToList()
        //     };

        //     return View(model);
        // }


        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> EditConnection(EditConnectionViewModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var user = await _userManager.GetUserAsync(User); // Current patient
        //         if (user == null)
        //         {
        //             return NotFound();
        //         }

        //         // Find the current connection
        //         var connection = await _context.PatientDoctors
        //             .FirstOrDefaultAsync(pd => pd.PatientId == user.Id && pd.DoctorId == model.OldDoctorId);

        //         if (connection != null)
        //         {
        //             // Update the connection to the new doctor
        //             connection.DoctorId = model.NewDoctorId;
        //             _context.Update(connection);
        //             await _context.SaveChangesAsync();

        //             TempData["SuccessMessage"] = "Connection updated successfully!";
        //             return RedirectToAction("ManageConnections");
        //         }
        //     }

        //     ModelState.AddModelError("", "Failed to update connection. Please try again.");
        //     return RedirectToAction("ManageConnections");
        // }


















    }
}