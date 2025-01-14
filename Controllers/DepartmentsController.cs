using System.Linq;
using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        public IActionResult Index()
        {
            var dep = _context.Departments.ToList();
            return View(dep);
        }

        // GET: Departments/Details/5
        public IActionResult Details(int id)
        {
            var dep = _context.Departments.FirstOrDefault(d => d.Id == id);
            if (dep == null)
                return NotFound();

            return View(dep);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Departments dep)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dep);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(dep);
        }

        // GET: Departments/Edit/5
        public IActionResult Edit(int id)
        {
            var dep = _context.Departments.FirstOrDefault(d => d.Id == id);
            if (dep == null)
                return NotFound();

            return View(dep);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Departments dep)
        {
            if (id != dep.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dep);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Departments.Any(d => d.Id == dep.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dep);
        }

        // GET: Departments/Delete/5
        public IActionResult Delete(int id)
        {
            var dep = _context.Departments.FirstOrDefault(d => d.Id == id);
            if (dep == null)
                return NotFound();

            return View(dep);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var dep = _context.Departments.FirstOrDefault(d => d.Id == id);
            if (dep == null)
                return NotFound();

            try
            {
                _context.Departments.Remove(dep);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Optional: Log the error (use a logging framework)
                ModelState.AddModelError(string.Empty, "Unable to delete. The department might be referenced by other entities.");
                return View(dep); // Return to the same view if deletion fails
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
