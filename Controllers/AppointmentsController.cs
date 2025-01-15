using System.Linq;
using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public IActionResult Index()
        {
            var appointments = _context.Appointments.ToList();
            return View(appointments);
        }

        // GET: Appointments/Details/5
        public IActionResult Details(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            return View();
        }

        
        // POST: Appointments/Create
// POST: Appointments/Create
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Appointment appointment)
{
    if (ModelState.IsValid)
    {
        _context.Add(appointment);
        _context.SaveChanges();

        // Set success message in TempData
        TempData["SuccessMessage"] = "Appointment created successfully!";

        // Stay on the Create page and show the message
        return View(); // Simply return the Create view again
    }
    return View(appointment);
}

       // GET: Appointments/Delete/5
public IActionResult Delete(int id)
{
    var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
    if (appointment == null)
        return NotFound();

    return View(appointment);
}

// POST: Appointments/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public IActionResult DeleteConfirmed(int id)
{
    var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
    if (appointment == null)
        return NotFound();

    try
    {
        _context.Appointments.Remove(appointment);
        _context.SaveChanges();
    }
    catch (DbUpdateException)
    {
        // Optional: Log the error (use a logging framework)
        ModelState.AddModelError(string.Empty, "Unable to delete. The appointment might be referenced by other entities.");
        return View(appointment); // Return to the same view if deletion fails
    }

    return RedirectToAction(nameof(Index)); // Redirect to the Index page after successful deletion
}

        }
    }

