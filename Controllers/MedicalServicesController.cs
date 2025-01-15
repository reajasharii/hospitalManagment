using System.Linq;
using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class MedicalServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalServicesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var services = _context.MedicalServices.ToList();
            return View(services);
        }


        public IActionResult Details(int id)
        {
            var service = _context.MedicalServices.Find(id);
            if (service == null)
                return NotFound();

            return View(service);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicalService service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }


        public IActionResult Edit(int id)
        {
            var service = _context.MedicalServices.Find(id);
            if (service == null)
                return NotFound();

            return View(service);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MedicalService service)
        {
            if (id != service.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MedicalServices.Any(e => e.Id == service.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }


        public IActionResult Delete(int id)
        {
            var service = _context.MedicalServices.FirstOrDefault(m => m.Id == id);
            if (service == null)
                return NotFound();

            return View(service);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var service = _context.MedicalServices.FirstOrDefault(m => m.Id == id);
            if (service == null)
                return NotFound();

            try
            {
                _context.MedicalServices.Remove(service);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete. The service might be referenced by other entities.");
                return View(service);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
