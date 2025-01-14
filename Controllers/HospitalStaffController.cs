using System.Linq;
using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class HospitalStaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HospitalStaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HospitalStaff
        public IActionResult Index()
        {
            var staff = _context.HospitalStaff.ToList();
            return View(staff);
        }

        // GET: HospitalStaff/Details/5
        public IActionResult Details(int id)
        {
            var staff = _context.HospitalStaff.Find(id);
            if (staff == null)
                return NotFound();

            return View(staff);
        }

        // GET: HospitalStaff/Create
        public IActionResult Create()
        {
            return View();
        }

      [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(HospitalStaff staff)
{
    if (ModelState.IsValid)
    {
        _context.Add(staff);  // Add the staff object to the context
        _context.SaveChanges();  // Save the changes to the database
        return RedirectToAction(nameof(Index));  // Redirect to the index page
    }
    return View(staff);  // If validation fails, return the view with the staff object
}


        // GET: HospitalStaff/Edit/5
        public IActionResult Edit(int id)
        {
            var staff = _context.HospitalStaff.Find(id);
            if (staff == null)
                return NotFound();

            return View(staff);
        }

        // POST: HospitalStaff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, HospitalStaff staff)
        {
            if (id != staff.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.HospitalStaff.Any(e => e.Id == staff.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

 // GET: HospitalStaff/Delete/5
public IActionResult Delete(int id)
{
    var staff = _context.HospitalStaff.FirstOrDefault(h => h.Id == id);
    if (staff == null)
        return NotFound();
    return View(staff);
}

// POST: HospitalStaff/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public IActionResult DeleteConfirmed(int id)
{
    var staff = _context.HospitalStaff.FirstOrDefault(h => h.Id == id);
    if (staff == null)
        return NotFound();

    try
    {
        _context.HospitalStaff.Remove(staff);
        _context.SaveChanges();
    }
    catch (DbUpdateException)
    {
        ModelState.AddModelError(string.Empty, "Unable to delete. The staff might be referenced by other entities.");
        return View(staff);
    }

    return RedirectToAction(nameof(Index));
}


    }
}
