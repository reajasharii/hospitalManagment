using System.Linq;
using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    public class AboutUsContentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutUsContentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AboutUsContent
        public IActionResult Index()
        {
            var content = _context.AboutUsContent.ToList();

            // Kontrollo nëse përdoruesi është admin
            var userRole = User.IsInRole("Admin");
            ViewBag.IsAdmin = userRole;

            return View(content);
        }

        // GET: AboutUsContent/Details/5
        public IActionResult Details(int id)
        {
            var content = _context.AboutUsContent.FirstOrDefault(c => c.Id == id);
            if (content == null)
                return NotFound();

            return View(content);
        }

        // GET: AboutUsContent/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: AboutUsContent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(AboutUsContent content)
        {
            if (ModelState.IsValid)
            {
                _context.Add(content);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(content);
        }

        // GET: AboutUsContent/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var content = _context.AboutUsContent.FirstOrDefault(c => c.Id == id);
            if (content == null)
                return NotFound();

            return View(content);
        }

        // POST: AboutUsContent/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, AboutUsContent content)
        {
            if (id != content.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    content.UpdatedAt = System.DateTime.Now;
                    _context.Update(content);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AboutUsContent.Any(c => c.Id == content.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(content);
        }

        // GET: AboutUsContent/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var content = _context.AboutUsContent.FirstOrDefault(c => c.Id == id);
            if (content == null)
                return NotFound();

            return View(content);
        }

        // POST: AboutUsContent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var content = _context.AboutUsContent.FirstOrDefault(c => c.Id == id);
            if (content == null)
                return NotFound();

            try
            {
                _context.AboutUsContent.Remove(content);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Nuk mund të fshihet. Përmbajtja mund të jetë përdorur diku tjetër.");
                return View(content);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
