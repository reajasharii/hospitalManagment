using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// [Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PatientController> _logger;
    

    public PatientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,ILogger<PatientController> logger)
    {
        _context = context;
        _userManager = userManager;
           _logger = logger;
    }

 
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
    
    var doctors = _context.Doctors
        .Where(d => string.IsNullOrEmpty(searchText) || d.FullName.Contains(searchText))
        .Select(d => new DoctorViewModel
        {
            Doctor = d,
            IsConnected = _context.PatientDoctors.Any(pd => pd.DoctorId == d.Id && pd.PatientId == _userManager.GetUserId(User))
        })
        .ToList();

    return View("ConnectToDoctor", doctors); 
}




[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ConnectToDoctorPost(string doctorId)  
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

[HttpGet]
public async Task<IActionResult> DisconnectFeedbackForm(string doctorId)
{
    var patient = await _userManager.GetUserAsync(User);
    if (patient == null)
    {
        return NotFound("Logged-in user not found.");
    }

    var doctor = await _context.Doctors
        .FirstOrDefaultAsync(d => d.Id == doctorId);

    if (doctor == null)
    {
        return NotFound("Doctor not found.");
    }

    var model = new FeedbackViewModel
    {
        DoctorId = doctor.Id,
        PatientId = patient.Id,
        DoctorFullName = doctor.FullName,
        PatientFullName = patient.FullName
    };

    return View(model);
}



    
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SubmitFeedback(FeedbackViewModel model)
{
    var patient = await _userManager.GetUserAsync(User);
    if (patient == null)
    {
        return NotFound("Logged-in user not found.");
    }

    var doctor = await _context.Doctors
        .FirstOrDefaultAsync(d => d.Id == model.DoctorId);

    if (doctor == null)
    {
        TempData["ErrorMessage"] = "Doctor not found.";
        return RedirectToAction("ConnectToDoctor");
    }

    var connection = await _context.PatientDoctors
        .FirstOrDefaultAsync(pd => pd.PatientId == patient.Id && pd.DoctorId == model.DoctorId);

    if (connection == null)
    {
        TempData["ErrorMessage"] = "You are not connected to this doctor.";
        return RedirectToAction("ConnectToDoctor");
    }

    var feedback = new Feedback
    {
        DoctorId = model.DoctorId,
        PatientId = patient.Id,
        FeedbackText = model.FeedbackText,
        SubmittedAt = DateTime.UtcNow,
        DoctorFullName = doctor.FullName, 
        PatientFullName = patient.FullName  
    };

    _context.Feedbacks.Add(feedback);


    _context.PatientDoctors.Remove(connection);

    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = $"Feedback submitted successfully. You have been disconnected from Dr. {doctor.FullName}, and your feedback has been saved.";

    return RedirectToAction("ConnectToDoctor");
}


    

public IActionResult ManageConnections()
{

    return View();
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
    var patientId = _userManager.GetUserId(User); 

   
    var connectedDoctors = await _context.PatientDoctors
        .Where(pd => pd.PatientId == patientId) 
        .Select(pd => pd.DoctorId) 
        .ToListAsync();

    
    var notes = await _context.Notes
        .Where(n => connectedDoctors.Contains(n.DoctorId) && n.PatientId == patientId) 
        .Select(n => new NoteViewModel
        {
            Content = n.Content,
            CreatedAt = n.CreatedAt
        })
        .ToListAsync();

    return View(notes);
}




    
}
